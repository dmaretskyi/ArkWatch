using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using ArkWatch.Storage;
using ArkWatch.UI.ViewModels;
using ArkWatch.UI.Views;
using ReactiveUI;
using Splat;

namespace ArkWatch.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetAssembly(typeof(App)));

            new MainView {ViewModel = new MainViewModel(new JsonStorageProvider("storage.json"))}.Show();
        }
    }
}
