using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Models.Database
{
    public class Contact
    {
        [PrimaryKey, AutoIncrement]
        public int localID {  get; set; }
        public int id { get; set; }
        public string username { get; set; }
        public string avatarPath { get; set; }
    }
}
