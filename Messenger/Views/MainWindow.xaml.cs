using Hardcodet.Wpf.TaskbarNotification;
using Messenger.ViewModels;
using Messenger.Views;
using System;
using System.Drawing;
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
using Messenger.Elements;

namespace Messenger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LoginVM vm;
        public bool forceClose = false;

        public MainWindow()
        {
            InitializeComponent();
            vm = new LoginVM();
            TryLogin();
            CreateTaskBarIcon();
        }

        private async void TryLogin()
        {
            try
            {
                if (await vm.CheckIfLoggedIn())
                    MainFrame.NavigationService.Navigate(new Messenger.Views.Messenger());
                else  
                    MainFrame.NavigationService.Navigate(new Login());
            }
            catch 
            {
                MainFrame.NavigationService.Navigate(new Login());
            }
        }

        private async void CreateTaskBarIcon()
        {
            var taskBarIcon = new ToolBarMenu();
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !forceClose;
            this.Hide();
        }
    }
}
