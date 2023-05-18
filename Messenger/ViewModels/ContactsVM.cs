﻿using Messenger.Models.Database;
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

        public async Task<List<Models.Contact>> GetContacts()
        {
            var contacts = await GetAllUsers();
            contacts.RemoveAt(contacts.IndexOf(contacts.FirstOrDefault(p => p.id == ChatController.instance.myID)));
            foreach (var user in contacts)
            {
                var avatar = await GetUserAvatar(user.id);
                var lastMessage = await GetLastMessage(user.id);

                if (lastMessage != null)
                    user.lastMessageTime = lastMessage.created_at;
                else
                    user.lastMessageTime = DateTime.MinValue;

                if (avatar != null)
                    user.avatar = avatar;
                else
                    user.avatar = Properties.Resources.DefaultAvatarPath;
            }
            contacts = contacts.OrderByDescending(p => p.lastMessageTime).ToList();
            return contacts;
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
