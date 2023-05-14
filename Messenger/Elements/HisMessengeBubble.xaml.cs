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
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class HisMessengeBubble : Grid
    {
        public string time { get; set; }
        public string text { get; set; }
        public string status { get; set;}
        public HisMessengeBubble()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
