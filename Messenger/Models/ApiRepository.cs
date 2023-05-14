﻿using Messenger.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Net.WebSockets;
using System.Threading;
using System.Diagnostics;
using Messenger.Models.Database;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Messenger.Models
{
    public class ApiRepository
    {
        private static ApiRepository _instance;
        public static ApiRepository instance 
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new ApiRepository();
                }
                return _instance; 
            } 
        }

        HttpClient client;
        ClientWebSocket ws = new ClientWebSocket();
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        SQLiteDb db;

        public ApiRepository()
        {
            client = new HttpClient();
            db = new SQLiteDb();
        }

        private async Task<HttpResponseMessage> NonTokenyzePost(string address, object data)
        {
            string json = JsonConvert.SerializeObject(data);
            StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(address, stringContent);

            return response;
        }

        private async Task<HttpResponseMessage> TokenyzePost(string address, object content)
        {
            string json = JsonConvert.SerializeObject(content);
            StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await db.GetAccessTokenAsync());

            var response = await client.PostAsync(address, stringContent);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if(await RefreshToken())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await db.GetAccessTokenAsync());
                    return await client.PostAsync(address, stringContent);

                }
            }

            return response;
        }

        private async Task<HttpResponseMessage> TokenyzeGet(string address)
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await db.GetAccessTokenAsync());

            var response = await client.GetAsync(address);

            if(response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if(await RefreshToken())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await db.GetAccessTokenAsync());
                    return await client.GetAsync(address);
                }
            }

            return response;
        }

        private async Task<bool> RefreshToken()
        {
            string refreshToken = await db.GetRefreshTokenAsync();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", refreshToken);
            var response = await client.PostAsync("https://bigeny.ru/api/auth/refresh", null);
            var tokens = JsonConvert.DeserializeObject<Tokens>(await response.Content.ReadAsStringAsync());
            var results = new List<ValidationResult>();
            var stringResponse = await response.Content.ReadAsStringAsync();

            if (!Validator.TryValidateObject(tokens, new ValidationContext(tokens), results, true))
                return false;

            db.SetTokensAsync(tokens.access_token, tokens.refresh_token);
            return true;
        }

        public async Task<Tokens> Login(string username, string password)
        {
            var data = new UserLogin(username, password);
            var response = await NonTokenyzePost("https://bigeny.ru/api/auth/login", data);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new Exception("Неверный логин или пароль");

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Ошибка входа");

            var responseString = await response.Content.ReadAsStringAsync();
            Tokens tokens = JsonConvert.DeserializeObject<Tokens>(responseString);
            return tokens;
        }

        public async Task<bool> Logout()
        {
            await TokenyzePost("https://bigeny.ru/apu/auth/logout", null);
            db.Clear();
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Выход из аккаунта", tokenSource.Token);
            return true;
        }

        public async Task<Tokens> SignUp(string username, string password)
        {
            var data = new UserLogin(username, password);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(data, new ValidationContext(data), results, true))
                throw new Exception(results[0].ErrorMessage);

            var response = await NonTokenyzePost("https://bigeny.ru/api/auth/signup", data);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(response.StatusCode.ToString());

            var responseString = await response.Content.ReadAsStringAsync();
            Tokens tokens = JsonConvert.DeserializeObject<Tokens>(responseString);

            return tokens;
        }

        public async void EditName(string newName)
        {
            var data = new UserLogin(newName, "");
            var results = new List<ValidationResult>();

            if(!Validator.TryValidateObject(data,new ValidationContext(data), results, false))
                throw new Exception(results[0].ErrorMessage);

            var response = await TokenyzePost("https://bigeny.ru/api/users/edit", data);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(response.StatusCode.ToString());
        }

        public async Task<string> GetMyName()
        {
            var response = await TokenyzeGet("https://bigeny.ru/api/users/me");
            
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Ошибка получения данных пользователя");

            var stringResponse = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(stringResponse);
            
            if (user != null)
            {
                db.SetMyID(user.id);
                return user.username;
            }
            return null;
        }

        public async Task<List<Contact>> GetAllUsers()
        {
            var response = await TokenyzeGet("https://bigeny.ru/api/users/all");

            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new Exception("Сессия устарела");

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                throw new Exception("Ошибка сервера");

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Ошибка получения контактов");

            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Contact>>(stringResponse);
        }

        public async Task<BitmapImage> GetAvatar(int userId)
        {
            var response = await TokenyzeGet("https://bigeny.ru/api/users/avatar/download?user_id=" + userId);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = await response.Content.ReadAsStreamAsync();
            bitmap.EndInit();
            return bitmap;
        }

        public async Task<List<Message>> GetMessages(int count, int recipientId)
        {
            var response = await TokenyzeGet($"https://bigeny.ru/api/messages/get?recipient_id={recipientId}&limit={count}");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Ошибка получения сообщений");
            string stringResponse = await response.Content.ReadAsStringAsync();
            var messages = JsonConvert.DeserializeObject<List<Message>>(stringResponse);
            return messages;
        }

        public async Task ConnectWebSocket()
        {
            if(ChatController.instance.myID == 0)
            {
                await GetMyName();
            }
            await ws.ConnectAsync(new Uri($"ws://109.174.29.40:8123/api/messages/ws/{ChatController.instance.myID}"), tokenSource.Token);
            ReceiveMessages();
        }

        public async void SendMessage(string messageText, int recipientId, int? groupId, int senderId)
        {
            var message = new MessageToSend();
            message.message_text = messageText;
            message.sender_id = senderId;
            message.group_id = groupId;
            message.recipient_id = recipientId;
            string data = JsonConvert.SerializeObject(message);

            ReconnectIfDisconnected();

            await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(data)), WebSocketMessageType.Text, true, tokenSource.Token);
        }

        private async void ReconnectIfDisconnected()
        {
            if(ws.State != WebSocketState.Open)
            {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Закрытие для перезапуска", tokenSource.Token);
                ConnectWebSocket();
            }
        }

        private async void ReceiveMessages()
        {
            while (true)
            {
                ReconnectIfDisconnected();
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
                var msg = await ws.ReceiveAsync(buffer, tokenSource.Token);
                if (msg == null)
                    break;
                var stringMsg = Encoding.UTF8.GetString(buffer.Array, 0, msg.Count);
                var json = JsonConvert.DeserializeObject<Message>(stringMsg);

                ChatController.instance.CallNewMessage(json);
            }
        }
    }
}
