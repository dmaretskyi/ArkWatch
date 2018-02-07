using System;
using System.Collections.Generic;
using ArkWatch.Models;
using ReactiveUI;

namespace ArkWatch.UI.ViewModels
{
    public class ActiveTribeViewModel : ActivePlayerGroupViewModel
    {
        public Tribe Tribe { get; }

        public ActiveTribeViewModel(Tribe tribe, IList<Player> playersOnline) : base(tribe.Name, playersOnline)
        {
            Tribe = tribe ?? throw new ArgumentNullException(nameof(tribe));
        }

        public string TribeName => Tribe.Name;

        public int NumberMembersTotal => Tribe.Members.Count;
    }
}