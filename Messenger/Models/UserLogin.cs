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
        [RegularExpression("[A-Za-z0-9]+", ErrorMessage = "В логине могут присутствовать только латинский буквы и цифры")]
        public string username { get; set; }

        [StringLength(30, MinimumLength = 4, ErrorMessage = "Минимальная длина пароля 4 символа, максимальная 30")]
        [CustomValidation(typeof(UserLogin), "ValidatePassword")]
        public string password { get; set; }

        public UserLogin(string username, string password)
        {
            this.username = username.Trim();
            this.password = password.Trim();
        }

        public static ValidationResult ValidatePassword(string password, ValidationContext context)
        {
            string regex1 = @"[A-Z]+";
            string regex2 = @"[a-z]+";
            string regex3 = @"[^a-zA-Z0-9]";

            if (!Regex.IsMatch(password, regex1))
                return new ValidationResult("В пароле должна присутствовать хотя бы одна заглавная буква");
            if (!Regex.IsMatch(password, regex2))
                return new ValidationResult("В пароле должна присутствовать хотя бы одна буква нижнего регистра");
            if (!Regex.IsMatch(password, regex3))
                return new ValidationResult("В пароле должен присутствовать спецсимвол");

            return ValidationResult.Success;
        }
    }
}
