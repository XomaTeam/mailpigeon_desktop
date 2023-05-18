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
            contacts = await vm.GetContacts(reloadAvatars: true);
            ContactsList.ItemsSource = contacts;
        }

        private async void OnNewMessage(Message msg)
        {
            contacts = await vm.GetContacts(reloadAvatars: false);
            ContactsList.ItemsSource = contacts;
        }

        private void Search(string search_name)
        {
            if(contacts.Count == 0)
                return; 

            if (string.IsNullOrEmpty(search_name))
            {
                ContactsList.ItemsSource = contacts;
                return;
            }

            var finded = from contact in contacts
                            where contact.username.ToLower().StartsWith(search_name.ToLower())
                            select contact;
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
