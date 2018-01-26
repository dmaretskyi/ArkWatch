using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ArkWatch.Models;
using ArkWatch.Storage;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ArkWatch.UI.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly IStorageProvider _storageProvider;

        private StorageData _data;

        private ReactiveList<Server> _servers;
        private ReactiveList<Tribe> _tribes;

        [Reactive]
        public IReactiveList<Server> Servers { get; private set; }


        [Reactive]
        public Server SelectedServer { get; set; }

        public ActivePlayersViewModel ServerInfo { [ObservableAsProperty] get; }

        public MainViewModel(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));

            _data = _storageProvider.LoadData();

            Servers = new ReactiveList<Server>(_data.Servers);

            this.WhenAnyValue(x => x.SelectedServer)
                .Where(server => server != null)
                .Select(server => new ActivePlayersViewModel(server))
                .ToPropertyEx(this, x => x.ServerInfo);
           
        }
    }
}