﻿using Messenger.Models;
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

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for Contacts.xaml
    /// </summary>
    public partial class Contacts : Page
    {
        ContactsVM vm = new ContactsVM();
        List<Contact> contacts;

        public Contacts()
        {
            InitializeComponent();
            SetContacts();
        }

        private async void SetContacts()
        {
            try
            {
                vm.UpdateContacts();
                contacts = await vm.GetAllUsers();
                contacts.RemoveAt(contacts.IndexOf(contacts.FirstOrDefault(p => p.id == ChatController.instance.myID)));
                foreach (var user in contacts)
                {
                        var avatar = await vm.GetUserAvatar(user.id);
                    if(avatar != null)
                        user.avatar = avatar;
                    else
                        user.avatar = Properties.Resources.DefaultAvatarPath;
                }
                ContactsList.ItemsSource = contacts;
            }
            catch(Exception ex) 
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Search(string search_name)
        {
            if (!string.IsNullOrEmpty(search_name))
            {
                var finded = from contact in contacts where contact.username.ToLower().StartsWith(search_name.ToLower()) select contact;
                ContactsList.ItemsSource = finded;
            }
            else
            {
                ContactsList.ItemsSource = contacts;
            }
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
            var tb = (TextBox)sender;
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
