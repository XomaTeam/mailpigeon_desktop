using Messenger.Models.Database;
using Messenger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Navigation;

namespace Messenger.ViewModels
{
    public class ContactsVM
    {
        ApiRepository api = ApiRepository.instance;
        SQLiteDb db;

        public ContactsVM()
        {
            db = new SQLiteDb();
        }

        public async Task<List<Models.Contact>> GetAllUsers()
        {
            var users = await api.GetAllUsers();
            await db.SetContacts(users);
            return users;
        }

        public async Task<BitmapImage> GetUserAvatar(int userId)
        {
            return await api.GetAvatar(userId);
        }

        public async void UpdateContacts()
        {
            api.UpdateSessionInfo();
        }

        public async Task<Message> GetLastMessage(int userID)
        {
            var message = await api.GetMessages(1, userID);
            if (message.Count > 0)
                return message[0];
            return null;
        }
    }
}
