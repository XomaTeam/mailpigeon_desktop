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
using System.Net.Http.Headers;
using System.Reflection;

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

        private async Task<HttpResponseMessage> TokenyzePostImage(string address, string filepath)
        {
            var content = new MultipartFormDataContent();
            var file = File.OpenRead(filepath);
            var streamContent = new StreamContent(file);
            var ext = Path.GetExtension(filepath).Remove(0,1);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue($"image/{ext}");
            content.Add(streamContent, "file", Path.GetFileName(filepath));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await db.GetAccessTokenAsync());
            var response = await client.PostAsync(address, content);
            string stringResponse = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                if (await RefreshToken())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await db.GetAccessTokenAsync());
                    return await client.PostAsync(address, content);

                }
            }

            return response;
        }

        private async Task<HttpResponseMessage> TokenyzePost(string address, object content)
        {
            string json = JsonConvert.SerializeObject(content);
            StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await db.GetAccessTokenAsync());

            var response = await client.PostAsync(address, stringContent);

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                if(await RefreshToken())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await db.GetAccessTokenAsync());
                    return await client.PostAsync(address, stringContent);

                }
            }

            return response;
        }

        private async Task<HttpResponseMessage> TokenyzeGet(string address)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await db.GetAccessTokenAsync());

            var response = await client.GetAsync(address);

            if(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                if(await RefreshToken())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await db.GetAccessTokenAsync());
                    return await client.GetAsync(address);
                }
            }

            return response;
        }

        private async Task<bool> RefreshToken()
        {
            string refreshToken = await db.GetRefreshTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", refreshToken);
            var response = await client.PostAsync($"{ApiAddresses.BASE_URL}/auth/refresh", null);
            var tokens = JsonConvert.DeserializeObject<Tokens>(await response.Content.ReadAsStringAsync());
            var results = new List<ValidationResult>();
            var stringResponse = await response.Content.ReadAsStringAsync();

            if (!Validator.TryValidateObject(tokens, new ValidationContext(tokens), results, true))
                return false;

            await db.SetTokensAsync(tokens.access_token, tokens.refresh_token);
            return true;
        }

        public async Task<Tokens> Login(string email, string password)
        {
            var data = new UserLogin(email, password);
            var response = await NonTokenyzePost($"{ApiAddresses.BASE_URL}/auth/login", data);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new Exception("Неверный логин или пароль");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Ошибка входа");

            var responseString = await response.Content.ReadAsStringAsync();
            Tokens tokens = JsonConvert.DeserializeObject<Tokens>(responseString);
            return tokens;
        }

        public async Task<bool> Logout()
        {
            await TokenyzePost($"{ApiAddresses.BASE_URL}/auth/logout", null);
            db.Clear();
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Выход из аккаунта", tokenSource.Token);
            ws.Dispose();
            return true;
        }

        public async Task<Tokens> SignUp(string name, string surname, string email, string password)
        {
            var data = new UserRegistration(name, surname, email, password);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(data, new ValidationContext(data), results, true))
                throw new Exception(results[0].ErrorMessage);

            var response = await NonTokenyzePost($"{ApiAddresses.BASE_URL}/auth/signup", data);

            if (!response.IsSuccessStatusCode)
                throw new Exception(response.StatusCode.ToString());

            var responseString = await response.Content.ReadAsStringAsync();
            Tokens tokens = JsonConvert.DeserializeObject<Tokens>(responseString);

            return tokens;
        }

        public async void EditName(string name, string surname, string email)
        {
            var data = new UserEdit(name, surname, email);
            var results = new List<ValidationResult>();

            if(!Validator.TryValidateObject(data,new ValidationContext(data), results, false))
                throw new Exception(results[0].ErrorMessage);

            var response = await TokenyzePost($"{ApiAddresses.BASE_URL}/users/edit", data);
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.StatusCode.ToString());

            var responseString = await response.Content.ReadAsStringAsync();
            Tokens tokens = JsonConvert.DeserializeObject<Tokens>(responseString);

            await db.SetTokensAsync(tokens.access_token, tokens.refresh_token);
        }
            
        public async Task<string> GetMyName()
        {
            var response = await TokenyzeGet($"{ApiAddresses.BASE_URL}/users/me");
            
            if (!response.IsSuccessStatusCode)
                throw new Exception("Ошибка получения данных пользователя");

            var stringResponse = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(stringResponse);
            
            if (user != null)
            {
                return user.name + " " + user.surname;
            }

            return null;
        }

        public async Task<User> GetUser()
        {
            var response = await TokenyzeGet($"{ApiAddresses.BASE_URL}/users/me");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Ошибка получения данных пользователя");

            var stringResponse = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(stringResponse);

            if (user != null)
            {
                return user;
            }

            return null;
        }

        public async Task UpdateSessionInfo()
        {
            var response = await TokenyzeGet($"{ApiAddresses.BASE_URL}/users/me");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Ошибка получения данных пользователя");

            var stringResponse = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(stringResponse);

            if (user != null)
            {
                await db.SetMyID(user.id);
                ChatController.instance.myID = user.id;
                ChatController.instance.myUsername = user.name + " " + user.surname;
            }
        }

        public async Task<List<Contact>> GetAllUsers()
        {
            var response = await TokenyzeGet($"{ApiAddresses.BASE_URL}/users/all");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Ошибка получения контактов");

            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Contact>>(stringResponse);
        }

        public async Task<List<Dialog>> GetAllDialogs()
        {
            var response = await TokenyzeGet($"{ApiAddresses.BASE_URL}/messages/dialogs");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Ошибка получения диалогов");

            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Dialog>>(stringResponse);
        }

        public async Task<BitmapImage> GetAvatar(int userId, bool forceReload)
        {
            try
            {
                var avatarPath = await db.GetAvatarPath(userId);
                if (avatarPath != null && !forceReload)
                {
                    var savedAvatar = new BitmapImage(new Uri(avatarPath));
                    return savedAvatar;
                }
                else
                {
                    var response = await TokenyzeGet($"{ApiAddresses.BASE_URL}/avatar/download?user_id=" + userId);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = await response.Content.ReadAsStreamAsync();
                    bitmap.EndInit();
                    await SaveAvatarToLocalStorage(bitmap, userId);
                    return bitmap;
                }
            }
            catch { return null; }
        }

        private async Task SaveAvatarToLocalStorage(BitmapImage image, int userId)
        {
            if (image == null)
                return;
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            string pathToFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "avatar" + userId);
            using (var fs = new FileStream(pathToFile, FileMode.Create))
            {
                encoder.Save(fs);
            }
            await db.SetOneContact(userId, pathToFile);
        }

        public async void SendAvatar(string filepath)
        {
            var response = await TokenyzePostImage($"{ApiAddresses.BASE_URL}/avatar/upload", filepath);
        }

        public async Task<List<Message>> GetMessages(int count, int offset, int recipientId)
        {
            var response = await TokenyzeGet($"{ApiAddresses.BASE_URL}/messages/get?recipient_id={recipientId}&limit={count}&offset={offset}");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Ошибка получения сообщений");

            string stringResponse = await response.Content.ReadAsStringAsync();
            var messages = JsonConvert.DeserializeObject<List<Message>>(stringResponse);
            return messages;
        }

        public async Task<User> GetUser(int userID)
        {
            var response = await TokenyzeGet($"{ApiAddresses.BASE_URL}/users/get?user_id={userID}");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Ошибка получения данных пользователя user_id=" + userID);
            string stringResponse = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(stringResponse);
            return user;

        }
         
        public async Task ConnectWebSocket()
        {
            ws = new ClientWebSocket();
            if(ChatController.instance.myID == 0)
            {
                await UpdateSessionInfo();
            }

            await ws.ConnectAsync(new Uri($"{ApiAddresses.BASE_WEB_SOCKET}/messages/ws/{ChatController.instance.myID}"), tokenSource.Token);

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
                //await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Закрытие для перезапуска", tokenSource.Token);
                await ConnectWebSocket();
            }
        }

        private async void ReceiveMessages()
        {
            while (true)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
                if (ws.State != WebSocketState.Open)
                    break;
                ReconnectIfDisconnected();
                var msg = await ws.ReceiveAsync(buffer, tokenSource.Token);
                var stringMsg = Encoding.UTF8.GetString(buffer.Array, 0, msg.Count);
                var json = JsonConvert.DeserializeObject<Message>(stringMsg);

                ChatController.instance.CallNewMessage(json);
            }
        }
    }
}
