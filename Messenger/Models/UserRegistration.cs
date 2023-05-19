using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Models
{
    public class UserRegistration
    {
        [Required(ErrorMessage = "Требуется имя")]
        [StringLength(100, ErrorMessage = "Максимальная длина имени 100 символов")]
        [RegularExpression("[А-Яа-я]+", ErrorMessage = "В логине могут присутствовать только латинские буквы, цифры и нижнее подчеркивание")]
        public string name { get; set; }

        [Required(ErrorMessage = "Требуется фамилия")]
        [StringLength(100, ErrorMessage = "Максимальная длина фамилии 100 символов")]
        [RegularExpression("[А-Яа-я]+", ErrorMessage = "В логине могут присутствовать только латинские буквы, цифры и нижнее подчеркивание")]
        public string surname { get; set; }

        [Required(ErrorMessage = "Требуется email")]
        [EmailAddress(ErrorMessage = "Не является email адресом")]
        public string email { get; set; }

        [Required(ErrorMessage = "Требуется пароль")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Минимальная длина пароля 8 символов, максимальная 30")]
        public string password { get; set; }

        public UserRegistration(string name, string surname, string email, string password)
        {
            this.name = name;
            this.surname = surname;
            this.password = password;
            this.email = email;
        }
    }
}
