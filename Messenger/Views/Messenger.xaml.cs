using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Messenger.Models;
using Messenger.Properties;
using Messenger.ViewModels;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for Messenger.xaml
    /// </summary>
    public partial class Messenger : Page
    {
        MessengerVM vm = new MessengerVM();
        LoginVM loginVM = new LoginVM();

        public Messenger()
        {
            InitializeComponent();
            vm.ConnectToMessagesStream();
            SideFrame.Navigate(new Contacts());
            ChatController.instance.SelectedDialogChanged += DialogChanged;
            ChatController.instance.NewMessage += ReceiveMessage;
            this.KeyUp += KeyUp_Clicked;
        }

        public async void DialogChanged(int newId)
        {
            Messages_lb.Items.Clear();
            await SetDialogUsername(newId);
            await CreateDialogMessages(newId);
            await SetDialogAvatar(newId);
        }

        private async Task SetDialogUsername(int userID)
        {
            Username_tb.Text = await vm.GetUserNickname(userID);
        }

        private async Task SetDialogAvatar(int userID)
        {
            var avatar = await vm.GetAvatar(userID);

            if (avatar != null)
                ContactAvatar.ImageSource = avatar;
            else
                ContactAvatar.ImageSource = new BitmapImage(new Uri(Properties.Resources.DefaultAvatarPath));
        }

        public async Task CreateDialogMessages(int id)
        {
            List<Message> messages = await vm.GetMessages(50, id);

            messages = messages.OrderByDescending(p => p.created_at).ToList();

            foreach (Message message in messages)
            {

                if (message.sender_id == ChatController.instance.myID)
                {
                    CreateMyMessage(message, false);
                }
                else
                {
                    CreateHisMessage(message, false);
                }
            }

            if (Messages_lb.Items.Count > 0)
                Messages_lb.ScrollIntoView(Messages_lb.Items[Messages_lb.Items.Count - 1]);
        }

        private void ReceiveMessage(Message msg)
        {
            if (msg == null)
                return;

            if (msg.sender_id == ChatController.instance.myID && msg.recipient_id == ChatController.instance.currentDialog)
            {
                CreateMyMessage(msg, true);
            }
            else if (msg.recipient_id == ChatController.instance.myID && msg.sender_id == ChatController.instance.currentDialog)
            {
                CreateHisMessage(msg, true);
            }

            if(Messages_lb.Items.Count > 0)
                Messages_lb.ScrollIntoView(Messages_lb.Items[Messages_lb.Items.Count - 1]);
        }

        private void CreateMyMessage(Message message, bool drawOnBottom)
        {
            var bubble = new MyMessengeBubble();
            bubble.text = message.message_text;
            bubble.HorizontalAlignment = HorizontalAlignment.Right;
            TimeZoneInfo zoneInfo = TimeZoneInfo.Local;
            DateTime time = TimeZoneInfo.ConvertTimeFromUtc(message.created_at, zoneInfo);
            bubble.time = time.ToString();
            if(drawOnBottom)
                Messages_lb.Items.Add(bubble);
            else
                Messages_lb.Items.Insert(0, bubble);
        }

        private void CreateHisMessage(Message message, bool drawOnBottom)
        {
            var bubble = new HisMessengeBubble();
            bubble.text = message.message_text;
            bubble.HorizontalAlignment = HorizontalAlignment.Left;
            TimeZoneInfo zoneInfo = TimeZoneInfo.Local;
            DateTime time = TimeZoneInfo.ConvertTimeFromUtc(message.created_at, zoneInfo);
            bubble.time = time.ToString();
            if (drawOnBottom)
                Messages_lb.Items.Add(bubble);
            else
                Messages_lb.Items.Insert(0, bubble);
        }

        private void Send_Clicked(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            if (Message_tb.Text.Trim().Length == 0)
                return;

            if (ChatController.instance.currentDialog == 0)
                return;

            vm.SendMessage(Message_tb.Text);
            Message_tb.Text = String.Empty;
        }

        private void Message_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if(textBox.Text.Length == 0)
            {
                MessageTip.Visibility = Visibility.Visible;
            }
            else
            {
                MessageTip.Visibility = Visibility.Hidden;
            }
        }

        private void KeyUp_Clicked(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                SendMessage();
        }

    }
}
