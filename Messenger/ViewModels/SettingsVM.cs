﻿using Messenger.Models.Database;
using Messenger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Messenger.ViewModels
{
    public class SettingsVM
    {
        ApiRepository api = ApiRepository.instance;
        SQLiteDb db;
        
        public SettingsVM()
        {
            db = new SQLiteDb();
        }

        public async Task<string> GetName()
        {
            return await api.GetMyName();
        }

        public async Task<bool> LogOut()
        {
            await api.Logout();
            return true;
        }
    }
}
