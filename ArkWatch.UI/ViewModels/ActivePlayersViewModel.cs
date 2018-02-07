using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ArkWatch.Models;
using ArkWatch.Storage;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ArkWatch.UI.ViewModels
{
    public class ActivePlayersViewModel : ReactiveObject
    {
        private readonly Server _server;
        private readonly IEnumerable<Tribe> _tribes;
        private readonly IEnumerable<Player> _players;

        private ServerInfo CurrentServerInfo { [ObservableAsProperty] get; }

        public IEnumerable<ActivePlayerGroupViewModel> GroupsOnline { [ObservableAsProperty] get; }

        private ReactiveCommand<Unit, ServerInfo> QueryServerInfo { get; }

        public string Status { [ObservableAsProperty] get; }

        private StorageData Data { [ObservableAsProperty] get; }
        
        public ActivePlayersViewModel(Server server, IObservable<StorageData> data)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));

            QueryServerInfo = ReactiveCommand.CreateFromTask(() => ServerQuery.ServerQuery.Query(_server.GetIpEndPoint()));

            data.ToPropertyEx(this, x => x.Data);

            Observable
                .Interval(TimeSpan.FromSeconds(5))
                .Select(_ => default(Unit))
                .InvokeCommand(QueryServerInfo);
            
            QueryServerInfo
                .ObserveOn(DispatcherScheduler.Current)
                .ToPropertyEx(this, x => x.CurrentServerInfo);

            Observable.Merge(
                QueryServerInfo
                    .ThrownExceptions
                    .Select(ex => $"Error: ex.Message"),
                this.WhenAnyValue(x => x.CurrentServerInfo)
                    .Select(info => info != null ? $"Players online: {info.Players.Count(player => !string.IsNullOrWhiteSpace(player.Name))}" : "Loading..")
            )
                .ToPropertyEx(this, x => x.Status);

            this.WhenAnyValue(x => x.CurrentServerInfo, x => x.Data)
                .Where(t => t.Item1 != null)
                .Select(t => SelectGroups(t.Item1.Players, t.Item2))
                .ToPropertyEx(this, x => x.GroupsOnline);
        }

        private IEnumerable<ActivePlayerGroupViewModel> SelectGroups(IEnumerable<PlayerInfo> onlinePlayers,
            StorageData data)
        {
            var players = SelectPlayers(onlinePlayers, data);

            List<ActivePlayerGroupViewModel> groups = data.Tribes
                .Select(tribe => Tuple.Create(tribe, players.Where(p => tribe.Members.Contains(p.Name)).ToList()))
                .Where(t => t.Item2.Count > 0)
                .Select(t => new ActiveTribeViewModel(t.Item1, t.Item2))
                .Cast<ActivePlayerGroupViewModel>()
                .ToList();

            var soloPlayers = players.Where(player => data.Tribes.All(tribe => !tribe.Members.Contains(player.Name)))
                .ToList();
            if (soloPlayers.Count > 0)
            {
                groups.Add(new ActivePlayerGroupViewModel("Solo players", soloPlayers));
            }

            return groups;
        }

        private IEnumerable<Player> SelectPlayers(IEnumerable<PlayerInfo> onlinePlayers, StorageData data)
        {
            var onlinePlayersList = onlinePlayers as IList<PlayerInfo> ?? onlinePlayers.ToList();
            var newPlayers = onlinePlayersList.Where(info => data.Players.All(p => p.Name != info.Name)).ToList();
            if(newPlayers.Count > 0) { 
                MessageBus.Current.SendMessage((IEnumerable<PlayerInfo>)newPlayers, "AddPlayers");
            }
            return onlinePlayersList.Except(newPlayers).Select(info => data.Players.First(p => p.Name == info.Name));
        }
    }
}