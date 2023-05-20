using Messenger.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Page
    {
        LoginVM vm;
        bool isPasswordVisible = false;
        bool isConfirmVisible = false;

        public Registration()
        {
            InitializeComponent();
            vm = new LoginVM();
        }

        //Переход в сам чат
        private async void RegBut_Click(object sender, RoutedEventArgs e)
        {
            if (TBPassword.Password != ConfirmPassword.Password)
                return;

            string password = isPasswordVisible ? PasswordVisible.Text : TBPassword.Password;

            try
            {
                if (await vm.SignUp(name_tb.Text, surname_tb.Text, email_tb.Text, password))
                    NavigationService.Navigate(new Messenger());
            }
            catch(Exception ex) { error_lbl.Text = ex.Message; }
        }

        //Отправка пользователя обратно на окно входа в приложение
        private void AuthBut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        

        private void email_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (email_tb.Text.Length == 0)
                emailTip.Visibility = Visibility.Visible;
            else
                emailTip.Visibility = Visibility.Hidden;
        }

        private void name_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (name_tb.Text.Length == 0)
                nameTip.Visibility = Visibility.Visible;
            else
                nameTip.Visibility = Visibility.Hidden;
        }

        private void surname_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (surname_tb.Text.Length == 0)
                surnameTip.Visibility = Visibility.Visible;
            else
                surnameTip.Visibility = Visibility.Hidden;
        }

        private void Password_Changed(object sender, RoutedEventArgs e)
        {
            if (TBPassword.Password.Length == 0)
                PasswordTip.Visibility = Visibility.Visible;
            else
                PasswordTip.Visibility = Visibility.Hidden;
        }

        private void PasswordVisible_Changed(object sender, TextChangedEventArgs e)
        {
            if (PasswordVisible.Text.Length == 0)
                PasswordTip.Visibility = Visibility.Visible;
            else
                PasswordTip.Visibility = Visibility.Hidden;
        }

        private void ConfirmVisible_Changed(object sender, TextChangedEventArgs e)
        {
            if (ConfirmVisible.Text.Length == 0)
                ConfirmPasswordTip.Visibility = Visibility.Visible;
            else
                ConfirmPasswordTip.Visibility = Visibility.Hidden;
        }

        private void PasswordConfirm_Changed(object sender, RoutedEventArgs e)
        {
            if (ConfirmPassword.Password.Length == 0)
                ConfirmPasswordTip.Visibility = Visibility.Visible;
            else
                ConfirmPasswordTip.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            isConfirmVisible = !isConfirmVisible;

            if (isConfirmVisible)
            {
                ConfirmVisible.Text = ConfirmPassword.Password;
                ConfirmPassword.Visibility = Visibility.Hidden;
                ConfirmVisible.Visibility = Visibility.Visible;
            }
            else
            {
                ConfirmPassword.Password = ConfirmVisible.Text;
                ConfirmPassword.Visibility = Visibility.Visible;
                ConfirmVisible.Visibility = Visibility.Hidden;
            }
        }


    }
}
