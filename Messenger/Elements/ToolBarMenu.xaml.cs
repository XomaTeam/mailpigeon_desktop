using Hardcodet.Wpf.TaskbarNotification;
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
using System.Windows.Shapes;

namespace Messenger.Elements
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class ToolBarMenu : TaskbarIcon
    {
        public ToolBarMenu()
        {
            InitializeComponent();
            this.TrayPopupOpen += LeftClick;
            this.IconSource = new BitmapImage(new Uri(Properties.Resources.AppIcon));
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).forceClose = true;
            Application.Current.MainWindow.Close();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();
        }

        private void LeftClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();
        }
    }
}
