using Messenger.Models.Database;
using Messenger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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

        public void EditName(string newName)
        {
            if (String.IsNullOrEmpty(newName))
                return;

            api.EditName(newName);
        }

        public void EditAvatar(byte[] avatar)
        {
            api.SendAvatar(avatar);
        }
    }
}
