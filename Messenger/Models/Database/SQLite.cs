using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Messenger.Models.Database
{
    public class SQLiteDb
    {
        SQLiteAsyncConnection conn;

        public SQLiteDb()
        {
            conn = new SQLiteAsyncConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "pegeo.db3"));
            conn.CreateTableAsync<Contact>().Wait();
            conn.CreateTableAsync<UserData>().Wait();
        }

        public async Task<bool> IsUserLoggedIn()
        {
            var users = await conn.Table<UserData>().ToListAsync();
            if (users.Count > 0)
                return true;
            return false;
        }

        public async Task SetTokensAsync(string access_token, string refresh_token)
        {
            var users = await conn.Table<UserData>().ToListAsync();
            if (users.Count == 0)
            {
                UserData user = new UserData(access_token, refresh_token);
                conn.InsertAsync(user).Wait();
            }
            else
            {
                UserData user = users[0];
                user.access_token = access_token;
                user.refresh_token = refresh_token;
                conn.UpdateAsync(user).Wait();
            }
        }

        public async void Clear()
        {
            await conn.Table<UserData>().Where(p => p.ID > 0).DeleteAsync();
        }

        public async Task<string> GetRefreshTokenAsync()
        {
            string refreshToken = (await conn.Table<UserData>().ToListAsync())[0].refresh_token;
            return refreshToken;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            try
            {
                string accessToken = (await conn.Table<UserData>().ToListAsync())[0].access_token;
                return accessToken;
            }
            catch { return null; }
        }

        public async Task<string> GetMyID()
        {
            string id = (await conn.Table<UserData>().ToListAsync())[0].userId;
            return id;
        }

        public async Task<string> GetMyUsername()
        {
            string username = (await conn.Table<UserData>().ToListAsync())[0].username;
            return username;
        }

        public async Task SetMyID(int id)
        {
            var users = await conn.Table<UserData>().ToListAsync();
            users[0].userId = id.ToString();
            conn.UpdateAsync(users[0]).Wait();
        }

        public async Task SetMyUsername(string username)
        {
            var users = await conn.Table<UserData>().ToListAsync();
            users[0].username = username;
            conn.UpdateAsync(users[0]).Wait();
        }

        public async Task SetContacts(List<Models.Contact> contacts)
        {
            foreach (var contact in contacts)
            {
                await SetOneContact(contact);
            }
        }

        public async Task SetOneContact(Models.Contact contact)
        {
            var contacts = await conn.Table<Contact>().ToListAsync();
            var existing = contacts.FirstOrDefault(p => p.id == contact.id);
            if (existing != null)
            {
                existing.username = contact.username;
                conn.UpdateAsync(existing).Wait();
            }
            else
            {
                var newContact = new Database.Contact();
                newContact.username = contact.username;
                newContact.id = contact.id;
                conn.InsertAsync(newContact).Wait();
            }
        }

        public async Task SetOneContact(Models.Contact contact, string avatarPath)
        {
            var contacts = await conn.Table<Contact>().ToListAsync();
            var existing = contacts.FirstOrDefault(p => p.id == contact.id);
            if (existing != null)
            {
                existing.username = contact.username;
                existing.avatarPath = avatarPath;
                conn.UpdateAsync(existing).Wait();
            }
            else
            {
                var newContact = new Database.Contact();
                newContact.username = contact.username;
                newContact.id = contact.id;
                newContact.avatarPath = avatarPath;
                conn.InsertAsync(newContact).Wait();
            }
        }

        public async Task SetOneContact(int contactID, string avatarPath)
        {
            var contacts = await conn.Table<Contact>().ToListAsync();
            var existing = contacts.FirstOrDefault(p => p.id == contactID);
            if (existing != null)
            {
                existing.avatarPath = avatarPath;
                conn.UpdateAsync(existing).Wait();
            }
            else
            {
                var newContact = new Database.Contact();
                newContact.id = contactID;
                newContact.avatarPath = avatarPath;
                conn.InsertAsync(newContact).Wait();
            }
        }

        public async Task<string> GetAvatarPath(int contactID)
        {
            var contacts = await conn.Table<Contact>().ToListAsync();
            var existing = contacts.FirstOrDefault(p => p.id == contactID);
            if(existing != null)
                return existing.avatarPath;
            else
                return null;
        }
    }
}
