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
using System.Windows.Shapes;
using ArkWatch.UI.ViewModels;
using ReactiveUI;

namespace ArkWatch.UI.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : IViewFor<MainViewModel>
    {
        public MainView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, vm => vm.Servers, v => v.ServerSelect.ItemsSource);
            this.Bind(ViewModel, vm => vm.SelectedServer, v => v.ServerSelect.SelectedItem);
            this.OneWayBind(ViewModel, vm => vm.ServerInfo, v => v.ActiveServerDisplay.ViewModel);
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(MainViewModel), typeof(MainView), new PropertyMetadata(default(MainViewModel)));

        public MainViewModel ViewModel
        {
            get { return (MainViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainViewModel) value; }
        }
    }
}
