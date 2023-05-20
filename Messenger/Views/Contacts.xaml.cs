using Messenger.Models;
using Messenger.Properties;
using Messenger.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for Contacts.xaml
    /// </summary>
    public partial class Contacts : Page
    {
        ContactsVM vm = new ContactsVM();
        List<Dialog> dialogs = new List<Dialog>();
        List<Contact> contacts = new List<Contact>();

        public Contacts()
        {
            InitializeComponent();
            SetContacts();
            ChatController.instance.NewMessage += OnNewMessage;
        }

        private async void SetContacts()
        {
            vm.UpdateContacts();
            dialogs = await vm.GetDialogs(reloadAvatars: true);
            foreach(var dialog in dialogs)
            {
                contacts.Add(dialog.recipient);
            }
            ContactsList.ItemsSource = contacts;
        }

        private async void OnNewMessage(Message msg)
        {
            // TODO: Изменить это на более эффективный вариант без запроса
            if (msg == null)
                return;

            dialogs = await vm.GetDialogs(reloadAvatars: false);
            ContactsList.ItemsSource = contacts;
        }

        private void Search(string search_name)
        {
            if(dialogs.Count == 0)
                return; 

            if (string.IsNullOrEmpty(search_name))
            {
                ContactsList.ItemsSource = contacts;
                return;
            }

            var finded = from dialog in dialogs
                            where dialog.recipient.username.ToLower().StartsWith(search_name.ToLower())
                            select dialog;
            ContactsList.ItemsSource = finded;
        }

        private void Hamburger_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SideMenu());
        }

        private void Contact_Clicked(object sender, MouseButtonEventArgs e)
        {
            var item = ContactsList.SelectedItem as Contact;
            if (item == null) 
                return;

            ChatController.instance.currentDialog = item.id;
        }

        private void Search_tb_Changed(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            Search(tb.Text);
            if (tb.Text.Length == 0)
            {
                SearchTip.Visibility = Visibility.Visible;
            }
            else
            {
                SearchTip.Visibility = Visibility.Hidden;
            }
        }
    }
}
