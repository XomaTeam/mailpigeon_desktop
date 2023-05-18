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
    public class MessengerVM
    {
        ApiRepository api = ApiRepository.instance;
        SQLiteDb db;

        public MessengerVM()
        {
            db = new SQLiteDb();
        }

        public async Task<List<Message>> GetMessages(int count, int recipientId)
        {
            return await api.GetMessages(count, recipientId);
        }

        public async void SendMessage(string message)
        {
            api.SendMessage(message, ChatController.instance.currentDialog, null, ChatController.instance.myID);
        }

        public void ConnectToMessagesStream()
        {
            api.ConnectWebSocket();
        }

        public async Task<string> GetUserNickname(int userID)
        {
            var user = await api.GetUser(userID);
            return user.username;
        }

        public async Task<BitmapImage> GetAvatar(int userID)
        {
            return await api.GetAvatar(userID, false);
        }
    }
}
