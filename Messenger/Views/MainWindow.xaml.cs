using Messenger.ViewModels;
using Messenger.Views;
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

namespace Messenger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LoginVM vm;
        public MainWindow()
        {
            InitializeComponent();
            vm = new LoginVM();
            TryLogin();
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
    }
}
