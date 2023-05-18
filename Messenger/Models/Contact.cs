using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Messenger.Models
{
    public class Contact
    {
        public int id {  get; set; }
        public string username { get; set; }
        public object avatar { get; set; }
        public DateTime lastMessageTime { get; set; }
    }
}
