using Messenger.Models.Database;
using Messenger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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

        public async Task<List<Contact>> GetAllUsers()
        {
            return await api.GetAllUsers();
        }

        public async Task<BitmapImage> GetUserAvatar(int userId)
        {
            return await api.GetAvatar(userId);
        }

        public async void UpdateContacts()
        {
            api.UpdateSessionInfo();
        }
    }
}
