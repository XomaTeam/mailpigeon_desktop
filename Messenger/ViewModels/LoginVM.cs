using Messenger.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Annotations;
using Messenger.Models.Database;
using System.IO;

namespace Messenger.ViewModels
{
    public class LoginVM
    {
        ApiRepository api = ApiRepository.instance;
        SQLiteDb db;
        public LoginVM()
        {
            db = new SQLiteDb();
        }

        public async Task<bool> LogIn(string username, string password)
        {
            Tokens tokens = await api.Login(username, password);
            List<ValidationResult> results = new List<ValidationResult>();
            bool result = Validator.TryValidateObject(tokens, new ValidationContext(tokens), results, true);
            if (result)
            {
                db.SetTokensAsync(tokens.access_token, tokens.refresh_token);
                await api.GetMyName();
            }
            return result;
        }

        public async Task<bool> SignUp(string username, string password)
        {
            Tokens tokens = await api.SignUp(username, password);
            List<ValidationResult> results = new List<ValidationResult>();
            bool result = Validator.TryValidateObject(tokens, new ValidationContext(tokens), results, true);
            if (result)
            {
                db.SetTokensAsync(tokens.access_token, tokens.refresh_token);
            }
            return result;
        }

        public async Task<bool> CheckIfLoggedIn()
        {
            if (await db.IsUserLoggedIn())
            {
                if(await api.GetMyName() != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
