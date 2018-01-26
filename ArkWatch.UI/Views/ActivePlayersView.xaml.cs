using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ArkWatch.UI.ViewModels;
using ReactiveUI;

namespace ArkWatch.UI.Views
{
    /// <summary>
    /// Interaction logic for ActivePlayersView.xaml
    /// </summary>
    public partial class ActivePlayersView : UserControl, IViewFor<ActivePlayersViewModel>
    {
        public ActivePlayersView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, vm => vm.PlayersOnline, v => v.PlayerOnline.ItemsSource);
            this.OneWayBind(ViewModel, vm => vm.Status, v => v.Status.Text);
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(ActivePlayersViewModel), typeof(ActivePlayersView), new PropertyMetadata(default(ActivePlayersViewModel)));

        public ActivePlayersViewModel ViewModel
        {
            get { return (ActivePlayersViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ActivePlayersViewModel) value; }
        }
    }
}
