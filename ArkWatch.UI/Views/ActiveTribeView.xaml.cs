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
    /// Interaction logic for ActiveTribeView.xaml
    /// </summary>
    public partial class ActiveTribeView : UserControl, IViewFor<ActiveTribeViewModel>
    {
        public ActiveTribeView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, vm => vm.TribeName, v => v.TribeName.Text);
            this.OneWayBind(ViewModel, vm => vm.NumberMembersOnline, v => v.NumberMemberOnline.Text);
            this.OneWayBind(ViewModel, vm => vm.NumberMembersTotal, v => v.NumberMemberTotal.Text);
            this.OneWayBind(ViewModel, vm => vm.MembersOnline, v => v.Members.ItemsSource);

        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(ActiveTribeViewModel), typeof(ActiveTribeView), new PropertyMetadata(default(ActiveTribeViewModel)));

        public ActiveTribeViewModel ViewModel
        {
            get { return (ActiveTribeViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ActiveTribeViewModel) value; }
        }
    }
}
