using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ArkWatch.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ArkWatch.UI.ViewModels
{
    public class ActivePlayersViewModel : ReactiveObject
    {
        private readonly Server _server;

        private ServerInfo CurrentServerInfo { [ObservableAsProperty] get; }

        public IEnumerable<PlayerInfo> PlayersOnline { [ObservableAsProperty] get; }

        private ReactiveCommand<Unit, ServerInfo> QueryServerInfo { get; }

        public string Status { [ObservableAsProperty] get; }
        
        public ActivePlayersViewModel(Server server)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));

            QueryServerInfo = ReactiveCommand.CreateFromTask(() => ServerQuery.ServerQuery.Query(_server.GetIpEndPoint()));

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

            this.WhenAnyValue(x => x.CurrentServerInfo)
                .Select(info => info?.Players?.Where(player => !string.IsNullOrWhiteSpace(player.Name)) ?? Enumerable.Empty<PlayerInfo>())
                .ToPropertyEx(this, x => x.PlayersOnline);
        }
    }
}