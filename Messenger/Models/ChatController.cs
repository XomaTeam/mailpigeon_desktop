using Messenger.Models.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Models
{
    public class ChatController
    {
        SQLiteDb db = new SQLiteDb();
        private static ChatController _instance;
        public static ChatController instance
        {
            get
            {
                if( _instance == null )
                    _instance = new ChatController();
                return _instance;
            }
        }

        public int myID;
        private string _myUsername;
        public string myUsername
        {
            set
            {
                _myUsername = value;
                UsernameChanged?.Invoke(value);
            }
            get { return _myUsername; }
        }
        private int _currentDialog;
        public int currentDialog
        {
            set
            {
                _currentDialog = value;
                SelectedDialogChanged?.Invoke(value);
            }
            get
            {
                return _currentDialog;
            }
        }
        public delegate void DialogHandler(int newId);
        public event DialogHandler SelectedDialogChanged;

        public delegate void MessageHandler(Message msg);
        public event MessageHandler NewMessage;

        public delegate void UsernameHandler(string username);
        public event UsernameHandler UsernameChanged;

        private ChatController() 
        {
            SetId();
        }

        private async void SetId()
        {
            myID = int.Parse(await db.GetMyID());
        }

        public void CallNewMessage(Message msg)
        {
            NewMessage?.Invoke(msg);
        }
    }
}
