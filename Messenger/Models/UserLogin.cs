using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Messenger.Models
{
    public class UserLogin
    {
        [EmailAddress(ErrorMessage = "Не является email адресом")]
        public string email { get; set; }

        [StringLength(30, MinimumLength = 8, ErrorMessage = "Минимальная длина пароля 8 символа, максимальная 30")]
        public string password { get; set; }

        public UserLogin(string email, string password)
        {
            this.email = email;
            this.password = password;
        }
    }
}
