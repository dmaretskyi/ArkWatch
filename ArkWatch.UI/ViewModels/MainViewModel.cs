using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ArkWatch.Models;
using ArkWatch.Storage;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ArkWatch.UI.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly IStorageProvider _storageProvider;

        [Reactive]
        private StorageData Data { get; set; }

        private ReactiveList<Server> _servers;
        private ReactiveList<Tribe> _tribes;

        public IEnumerable<Server> Servers { [ObservableAsProperty] get; }

        [Reactive]
        public Server SelectedServer { get; set; }

        public ActivePlayersViewModel ServerInfo { [ObservableAsProperty] get; }

        public MainViewModel(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));

            Data = _storageProvider.LoadData();

            this.WhenAnyValue(x => x.Data)
                .Select(data => data.Servers)
                .ToPropertyEx(this, x => x.Servers);

            this.WhenAnyValue(x => x.SelectedServer)
                .Where(server => server != null)
                .Select(server => new ActivePlayersViewModel(server, this.WhenAnyValue(x => x.Data)))
                .ToPropertyEx(this, x => x.ServerInfo);

            MessageBus.Current.Listen<IEnumerable<PlayerInfo>>("AddPlayers")
                .Subscribe(players =>
                {
                    foreach (var player in players)
                    {
                        Data.Players.Add(new Player(player.Name, ""));
                    }
                    _storageProvider.SaveData(Data);
                    Data = _storageProvider.LoadData();
                });

        }
    }
}