using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Models
{
    public  class Dialog
    {
        public Contact recipient { get; set; }
        public Message last_message { get; set; }
        public int count_unread_messages { get; set; }
    }
}
