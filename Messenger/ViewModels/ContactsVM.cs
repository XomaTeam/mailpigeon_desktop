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
using Messenger.Views;
using System.Data.Entity.Infrastructure;

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

        public async Task<List<Models.Dialog>> GetAllDialogs()
        {
            var dialogs = await api.GetAllDialogs();
            var contacts = new List<Models.Contact>();
            foreach (var dialog in dialogs)
            {
                contacts.Add(dialog.recipient);
            }
            await db.SetContacts(contacts);
            return dialogs;
        }

        public async Task<List<Dialog>> GetDialogs(bool reloadAvatars)
        {
            var dialogs = await GetAllDialogs();

            foreach (var dialog in dialogs)
            {
                var avatar = await GetUserAvatar(dialog.recipient.id, reloadAvatars);
                var lastMessage = dialog.last_message;
                dialog.recipient.CreateUsername();

                if (avatar != null)
                    dialog.recipient.avatar = avatar;
                else
                    dialog.recipient.avatar = Properties.Resources.DefaultAvatarPath;
            }

            dialogs = dialogs.OrderByDescending(p => p.last_message.created_at).ToList();
            return dialogs;
        }

        public async Task<BitmapImage> GetUserAvatar(int userId, bool forceReload)
        {
            return await api.GetAvatar(userId, forceReload);
        }

        public async void UpdateContacts()
        {
            api.UpdateSessionInfo();
        }

        public async Task<Message> GetLastMessage(int userID)
        {
            var message = await api.GetMessages(1, 0, userID);
            if (message.Count > 0)
                return message[0];
            return null;
        }
    }
}
