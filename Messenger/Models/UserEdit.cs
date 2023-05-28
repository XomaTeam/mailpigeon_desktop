using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Models
{
    internal class UserEdit
    {
        [RegularExpression("[А-Яа-я]+")]
        public string name {  get; set; }
        [RegularExpression("[А-Яа-я]+")]
        public string surname { get; set; }
        [EmailAddress]
        public string email { get; set; }

        public UserEdit(string name, string surname, string email)
        {
            this.name = name;
            this.surname = surname;
            this.email = email;
        }
    }
}
