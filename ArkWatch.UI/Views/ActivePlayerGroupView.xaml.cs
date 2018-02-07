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
    /// Interaction logic for ActivePlayerGroupView.xaml
    /// </summary>
    public partial class ActivePlayerGroupView : UserControl, IViewFor<ActivePlayerGroupViewModel>
    {
        public ActivePlayerGroupView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, vm => vm.Name, v => v.GroupName.Text);
            this.OneWayBind(ViewModel, vm => vm.NumberMembersOnline, v => v.NumberMemberOnline.Text);
            this.OneWayBind(ViewModel, vm => vm.MembersOnline, v => v.Members.ItemsSource);
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(ActivePlayerGroupViewModel), typeof(ActivePlayerGroupView), new PropertyMetadata(default(ActivePlayerGroupViewModel)));

        public ActivePlayerGroupViewModel ViewModel
        {
            get { return (ActivePlayerGroupViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ActivePlayerGroupViewModel) value; }
        }
    }
}
