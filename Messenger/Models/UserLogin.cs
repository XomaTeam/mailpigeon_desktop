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
        [Required(ErrorMessage = "Требуется логин")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Минимальная длина логина 5 символов, максимальная 50")]
        [RegularExpression("[A-Za-z0-9_]+", ErrorMessage = "В логине могут присутствовать только латинские буквы, цифры и нижнее подчеркивание")]
        public string username { get; set; }

        [Required(ErrorMessage = "Требуется пароль")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Минимальная длина пароля 8 символа, максимальная 30")]
        public string password { get; set; }

        public UserLogin(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
