using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Models.Database
{
    public class UserData
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string username { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string userId { get; set; }

        public UserData(string access_token, string refresh_token, string userID)
        {
            this.access_token = access_token;
            this.refresh_token = refresh_token;
            this.userId = userID;
        }

        public UserData(string access_token, string refresh_token)
        {
            this.access_token = access_token;
            this.refresh_token = refresh_token;
        }

        public UserData(string access_token, string refresh_token, string userID, string username)
        {
            this.access_token = access_token;
            this.refresh_token = refresh_token;
            this.userId = userID;
            this.username = username;
        }

        public UserData() { }
    }
}
