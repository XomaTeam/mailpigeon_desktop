using Messenger.Models;
using Messenger.ViewModels;
using System;
using System.Collections.Generic;
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
        

        public Contacts()
        {
            InitializeComponent();
            SetContacts();
        }

        private async void SetContacts()
        {
            try
            {
                List<Contact> contacts = await vm.GetAllUsers();
                foreach (var user in contacts)
                {
                    try
                    {
                        var avatar = await vm.GetUserAvatar(user.id);
                        user.avatar = avatar;
                    }
                    catch
                    {
                        user.avatar = "pack://application:,,,/Resources/Images/user.png";
                    }

                }
                ContactsList.ItemsSource = contacts;
            }
            catch(Exception ex) 
            {
                Debug.WriteLine(ex.Message);
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
    }
}
