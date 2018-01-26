using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ArkWatch.Models;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Checksums;

namespace ArkWatch.ServerQuery
{
    public static class ServerQuery
    {
        [Serializable]
        private enum ServerType : byte
        {
            Listen = 0x6C, //'l'
            Dedicated = 0x64,// 'd'
            SourceTv = 0x70,// 'p'
        }

        [Serializable]
        private enum OperatingSystem : byte
        {
            Linux = 0x6C,
            Windows = 0x77
        }

        [Flags]
        [Serializable]
        private enum ExtraDataFlags : byte
        {
            GamePort = 0x80,
            SpectatorInfo = 0x40,
            GameTagData = 0x20,
            SteamID = 0x10,
            GameID = 0x01,
        }

        private static readonly byte[] A2S_INFO = { 0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79, 0x00 };
        private static readonly byte[] A2S_SERVERQUERY_GETCHALLENGE = { 0x55, 0xFF, 0xFF, 0xFF, 0xFF };
        private static readonly byte A2S_PLAYER = 0x55;
        private static readonly byte A2S_RULES = 0x56;

        public static async Task<ServerInfo> Query(IPEndPoint address)
        {
            try
            {
                using (var client = new UdpClient())
                {
                    client.Client.SendTimeout = (int)500;
                    client.Client.ReceiveTimeout = (int)500;
                    client.Connect(address);

                    byte[] challengeBytes;

                    byte networkVersion;
                    string name;
                    string map;
                    string gameDirectory;
                    string gameDescription;
                    short appId;
                    byte playerCount;
                    byte maximumPlayerCount;
                    byte botCount;
                    ServerType serverType;
                    OperatingSystem os;
                    bool requiresPassword;
                    bool vacSecured;
                    string gameVersion;
                    short port = 27015;
                    string steamId;
                    short spectatorPort;
                    string spectatorName;
                    string gameTagData;
                    string gameId;

                    await Send(client, A2S_INFO);
                    var infoData = await Receive(client, address);
                    using (var br = new BinaryReader(new MemoryStream(infoData)))
                    {
                        br.ReadByte(); // type byte, not needed

                        networkVersion = br.ReadByte();
                        name = br.ReadAnsiString();
                        map = br.ReadAnsiString();
                        gameDirectory = br.ReadAnsiString();
                        gameDescription = br.ReadAnsiString();
                        appId = br.ReadInt16();
                        playerCount = br.ReadByte();
                        maximumPlayerCount = br.ReadByte();
                        botCount = br.ReadByte();
                        serverType = (ServerType)br.ReadByte();
                        os = (OperatingSystem)br.ReadByte();
                        requiresPassword = br.ReadByte() == 0x01;
                        vacSecured = br.ReadByte() == 0x01;
                        gameVersion = br.ReadAnsiString();
                        var edf = (ExtraDataFlags)br.ReadByte();

                        if (edf.HasFlag(ExtraDataFlags.GamePort)) port = br.ReadInt16();
                        if (edf.HasFlag(ExtraDataFlags.SteamID)) steamId = br.ReadUInt64().ToString();
                        if (edf.HasFlag(ExtraDataFlags.SpectatorInfo))
                        {
                            spectatorPort = br.ReadInt16();
                            spectatorName = br.ReadAnsiString();
                        }
                        if (edf.HasFlag(ExtraDataFlags.GameTagData)) gameTagData = br.ReadAnsiString();
                        if (edf.HasFlag(ExtraDataFlags.GameID)) gameId = br.ReadUInt64().ToString();
                    }

                    await Send(client, A2S_SERVERQUERY_GETCHALLENGE);
                    challengeBytes = await Receive(client, address);
                    if (challengeBytes[0] != 0x41) throw new Exception("Unable to retrieve challenge data");

                    challengeBytes[0] = A2S_PLAYER;
                    await Send(client, challengeBytes);

                    var playerData = await Receive(client, address);

                    var players = new List<PlayerInfo>();

                    using (var br = new BinaryReader(new MemoryStream(playerData)))
                    {
                        if (br.ReadByte() != 0x44) throw new Exception("Invalid data received in response to A2S_PLAYER request");
                        var numPlayers = br.ReadByte();
                        for (int index = 0; index < numPlayers; index++)
                        {
                            byte idx = br.ReadByte();
                            var playerName = br.ReadAnsiString();
                            var playerScore = br.ReadInt32();
                            var playerTimeConnected = TimeSpan.FromSeconds(br.ReadSingle());
                            players.Add(new PlayerInfo(playerName));
                        }
                    }

                    return new ServerInfo(name, players);
                }
            }
            catch (Exception e)
            {
                throw new ServerQueryException("Server query exception", e);
            }
        }
        private static async Task Send(UdpClient client, byte[] message)
        {
            var fullmessage = new byte[4 + message.Length];
            fullmessage[0] = fullmessage[1] = fullmessage[2] = fullmessage[3] = 0xFF;

            Buffer.BlockCopy(message, 0, fullmessage, 4, message.Length);
            await client.SendAsync(fullmessage, fullmessage.Length);
        }

        private static async Task<byte[]> Receive(UdpClient client, IPEndPoint endpoint)
        {
            byte[][] packets = null;
            byte packetNumber = 0, packetCount = 1;
            bool usesBzip2 = false;
            int crc = 0;

            do
            {
                var result = await client.ReceiveAsync();
                using (var br = new BinaryReader(new MemoryStream(result.Buffer)))
                {
                    if (br.ReadInt32() == -2)
                    {
                        int requestId = br.ReadInt32();
                        usesBzip2 = (requestId & 0x80000000) == 0x80000000;
                        packetNumber = br.ReadByte();
                        packetCount = br.ReadByte();
                        int splitSize = br.ReadInt32();

                        if (usesBzip2 && packetNumber == 0)
                        {
                            int decompressedSize = br.ReadInt32();
                            crc = br.ReadInt32();
                        }
                    }

                    if (packets == null) packets = new byte[packetCount][];

                    var data = new byte[result.Buffer.Length - br.BaseStream.Position];
                    Buffer.BlockCopy(result.Buffer, (int)br.BaseStream.Position, data, 0, data.Length);
                    packets[packetNumber] = data;
                }
            } while (packets.Any(p => p == null));

            var combinedData = Combine(packets);
            if (usesBzip2)
            {
                combinedData = Decompress(combinedData);
                Crc32 crc32 = new Crc32();
                crc32.Update(combinedData);
                if (crc32.Value != crc) throw new Exception("Invalid CRC for compressed packet data");
                return combinedData;
            }
            return combinedData;
        }

        private static byte[] Decompress(byte[] combinedData)
        {
            using (var compressedData = new MemoryStream(combinedData))
            using (var uncompressedData = new MemoryStream())
            {
                BZip2.Decompress(compressedData, uncompressedData, true);
                return uncompressedData.ToArray();
            }
        }

        private static byte[] Combine(byte[][] arrays)
        {
            var rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        private static string ReadAnsiString(this BinaryReader br)
        {
            var stringBytes = new List<byte>();
            byte charByte;
            while ((charByte = br.ReadByte()) != 0)
            {
                stringBytes.Add(charByte);
            }
            return Encoding.ASCII.GetString(stringBytes.ToArray());
        }
    }
}
