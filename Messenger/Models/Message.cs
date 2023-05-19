using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Models
{
    public class Message
    {
        public int? id { get; set; }
        public int sender_id { get; set; }
        public int recipient_id { get; set; }
        public int? group_id { get; set; }
        public string message_text { get; set; }
        public DateTime created_at { get; set; }
        public bool readed { get; set; }
    }
}
