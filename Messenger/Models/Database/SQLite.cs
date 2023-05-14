using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                        "db.db3"));
            conn.CreateTableAsync<UserData>().Wait();
        }

        public async Task<bool> IsUserLoggedIn()
        {
            var users = await conn.Table<UserData>().ToListAsync();
            if(users.Count > 0)
                return true;
            return false;
        }

        public async void SetTokensAsync(string access_token, string refresh_token)
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

        public async void SetMyID(int id)
        {
            var users = await conn.Table<UserData>().ToListAsync();
            users[0].userId = id.ToString();
            conn.UpdateAsync(users[0]).Wait();
        }
    }
}
