using Messenger.Models.Database;
using Messenger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;

namespace Messenger.ViewModels
{
    public class EditVM
    {
        ApiRepository api = ApiRepository.instance;
        SQLiteDb db;

        public EditVM()
        {
            db = new SQLiteDb();
        }

        public void EditName(string name, string surname, string email)
        {
            api.EditName(name, surname, email);
        }

        public void EditAvatar(string filepath)
        {
            api.SendAvatar(filepath);
        }

        public async Task<User> GetUser()
        {
            var user = await api.GetUser();
            return user;
        }

        public async Task<BitmapImage> GetMyAvatar()
        {
            return await api.GetAvatar(ChatController.instance.myID, false);
        }
    }
}
