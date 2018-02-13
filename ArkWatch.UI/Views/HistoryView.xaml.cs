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
using LiveCharts;
using ReactiveUI;

namespace ArkWatch.UI.Views
{
    /// <summary>
    /// Interaction logic for HistoryView.xaml
    /// </summary>
    public partial class HistoryView : Window, IViewFor<HistoryViewModel>
    {
        public HistoryView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, vm => vm.Servers, v => v.ServerSelect.ItemsSource);
            this.Bind(ViewModel, vm => vm.SelectedServer, v => v.ServerSelect.SelectedItem);
            this.OneWayBind(ViewModel, vm => vm.Chart, v => v.Chart.Series);
            this.OneWayBind(ViewModel, vm => vm.Labels, v => v.XAxis.Labels);
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(HistoryViewModel), typeof(HistoryView), new PropertyMetadata(default(HistoryViewModel)));

        public HistoryViewModel ViewModel
        {
            get { return (HistoryViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (HistoryViewModel) value; }
        }
    }
}
