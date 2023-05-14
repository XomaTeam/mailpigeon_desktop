using Messenger.ViewModels;
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
using static Messenger.Views.Login;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        SettingsVM vm;

        public Settings()
        {
            InitializeComponent();
            vm = new SettingsVM();
            SetName();
        }

        private async void SetName()
        {
            Username_tb.Text = await vm.GetName();
        }

        private void BackBut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SideMenu());
        }

        private void EditBut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Edit());
        }

        private async void LogoutBut_Click(object sender, RoutedEventArgs e)
        {
            await vm.LogOut();
            var pastWindow = Application.Current.MainWindow;
            Application.Current.MainWindow = new MainWindow();
            Application.Current.MainWindow.Show();
            pastWindow.Close();
            //NavigationService.Navigate(new Login());
        }
    }
}
