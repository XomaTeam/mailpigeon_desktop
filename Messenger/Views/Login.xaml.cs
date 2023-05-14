using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
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
using System.Data.SQLite;
using Messenger.ViewModels;
using System.Diagnostics;
using Messenger.Models;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        bool isPasswordVisible = false;
        LoginVM vm;

        public Login()
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
                    NavigationService.Navigate(new Messenger());
            }
            catch (Exception ex) 
            {
                error_lbl.Content = ex.Message;                
            }
        }

        //Переход на страницу регистрации
        private void RegBut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Registration());
        }

        //Переход в сам мессенджер
        private async void AuthBut_Click(object sender, RoutedEventArgs e)
        {
            var password = isPasswordVisible ? PasswordVisible.Text : TBPassword.Password;
            bool result = false;
            try
            {
                result = await vm.LogIn(TBLogin.Text, password);
            }
            catch(Exception ex)
            {
                error_lbl.Content = ex.Message;
            }

            if (result)
            {
                NavigationService.Navigate(new Messenger());
            }
        }

        private void TBLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            if(tb.Text.Length == 0)
                LoginTip.Visibility = Visibility.Visible;
            else
                LoginTip.Visibility = Visibility.Hidden;
        }

        private void TBPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var tb = (PasswordBox)sender;
            if (tb.Password.Length == 0)
                PasswordTip.Visibility = Visibility.Visible;
            else
                PasswordTip.Visibility = Visibility.Hidden;
        }

        private void PasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb.Text.Length == 0)
                PasswordTip.Visibility = Visibility.Visible;
            else
                PasswordTip.Visibility = Visibility.Hidden;
        }

        private void ShowPassword_btn_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;
            if (isPasswordVisible)
            {
                PasswordVisible.Text = TBPassword.Password;
                TBPassword.Visibility = Visibility.Hidden;
                PasswordVisible.Visibility = Visibility.Visible;
            }
            else
            {
                TBPassword.Password = PasswordVisible.Text;
                TBPassword.Visibility = Visibility.Visible;
                PasswordVisible.Visibility = Visibility.Hidden;
            }
        }
    }
}
