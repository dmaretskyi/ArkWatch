using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArkWatch.Models;
using Newtonsoft.Json;

namespace ArkWatch.Storage
{
    public class JsonStorageProvider : IStorageProvider
    {
        public string FilePath { get; }

        public JsonStorageProvider(string filePath)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public StorageData LoadData()
        {
            if (!File.Exists(FilePath))
            {
                SaveData(new StorageData(new List<Server>(), new List<Player>(), new List<Tribe>()));
            }

            var str = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<StorageData>(str);
        }

        public void SaveData(StorageData data)
        {
            var str = JsonConvert.SerializeObject(data);
            File.WriteAllText(FilePath, str);
        }
    }
}
