using Messenger.Models;
using Messenger.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for Edit.xaml
    /// </summary>
    public partial class Edit : Page
    {
        bool isAvatarChanged = false;
        string newAvatarPath = string.Empty;
        EditVM vm = new EditVM();
        public Edit()
        {
            InitializeComponent();
            Loaded += On_Loaded;
            Unloaded += On_Unloaded;
        }

        private void BackBut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Debug.WriteLine(newAvatarPath);
                if(isAvatarChanged)
                    vm.EditAvatar(newAvatarPath);

                if (!String.IsNullOrEmpty(Name_tb.Text))
                    vm.EditName(Name_tb.Text);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Name_tb_Changed(object sender, TextChangedEventArgs e)
        {

        }

        private void ChangeAvatar_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                isAvatarChanged = true;
                MyAvatar.ImageSource = new BitmapImage(new Uri(dlg.FileName));
                newAvatarPath = dlg.FileName;
            }
        }

        private void On_Unloaded(object sender, RoutedEventArgs e)
        {
            isAvatarChanged = false;
        }

        private async void On_Loaded(object sender, RoutedEventArgs e)
        {
            var avatar = await vm.GetMyAvatar();
            if (avatar != null)
                MyAvatar.ImageSource = avatar;
            else
                MyAvatar.ImageSource = new BitmapImage(new Uri(Properties.Resources.DefaultAvatarPath));
        }
    }
}
