using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Models
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string username { get; set; }    

        public User(int id, string name, string surname, string email)
        {
            this.id = id;
            this.name = name;
            this.surname = surname;
            this.email = email;
            this.username = name + " " + surname;
        }
    }
}
