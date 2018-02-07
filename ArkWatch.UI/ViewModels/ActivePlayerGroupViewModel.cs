using System;
using System.Collections.Generic;
using ArkWatch.Models;
using ReactiveUI;

namespace ArkWatch.UI.ViewModels
{
    public class ActivePlayerGroupViewModel : ReactiveObject
    {
        public string Name { get; }

        public IList<Player> MembersOnline { get; }

        public int NumberMembersOnline => MembersOnline.Count;

        public ActivePlayerGroupViewModel(string name, IList<Player> membersOnline)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            MembersOnline = membersOnline ?? throw new ArgumentNullException(nameof(membersOnline));
        }
    }
}