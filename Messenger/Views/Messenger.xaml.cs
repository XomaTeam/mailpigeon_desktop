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
using Messenger.ViewModels;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for Messenger.xaml
    /// </summary>
    public partial class Messenger : Page
    {
        MessengerVM vm = new MessengerVM();

        public Messenger()
        {
            InitializeComponent();
            SideFrame.Navigate(new Contacts());
            ChatController.instance.SelectedDialogChanged += DialogChanged;
            ChatController.instance.NewMessage += ReceiveMessage;
            vm.ConnectToMessagesStream();
        }

        public async void DialogChanged(int newId)
        {
            Messages_lb.Items.Clear();
            List<Message> messages = await vm.GetMessages(50, newId);
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
        }

        private void ReceiveMessage(Message msg)
        {
            if (msg.sender_id == ChatController.instance.myID)
            {
                CreateMyMessage(msg, true);
            }
            else if (msg.recipient_id == ChatController.instance.myID)
            {
                CreateHisMessage(msg, true);
            }
        }

        private void CreateMyMessage(Message message, bool drawOnBottom)
        {
            var bubble = new MyMessengeBubble();
            bubble.text = message.message_text;
            bubble.HorizontalAlignment = HorizontalAlignment.Right;
            bubble.time = message.created_at.ToString();
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
            bubble.time = message.created_at.ToString();
            if (drawOnBottom)
                Messages_lb.Items.Add(bubble);
            else
                Messages_lb.Items.Insert(0, bubble);
        }

        private void Send_Clicked(object sender, RoutedEventArgs e)
        {
            if (Message_tb.Text.Trim().Length == 0)
                return;
            
            vm.SendMessage(Message_tb.Text);
        }
    }
}
