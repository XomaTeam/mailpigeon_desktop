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

        public async Task<List<Message>> GetMessages(int count, int offset, int recipientId)
        {
            return await api.GetMessages(count, offset, recipientId);
        }

        public async Task<List<Message>> GetMessages(int count, int recipientId, int offset)
        {
            return await api.GetMessages(count, recipientId, offset);
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
            return user.name + " " + user.surname;
        }

        public async Task<BitmapImage> GetAvatar(int userID)
        {
            return await api.GetAvatar(userID, false);
        }

        public async Task<List<Message>> GetExtraMessagesInCurrentDialog(int actualMessagesCount)
        {
            if(actualMessagesCount < 50)
                return null;

            return await api.GetMessages(50, actualMessagesCount, ChatController.instance.currentDialog);
        }
    }
}
