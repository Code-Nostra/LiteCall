using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using ServerSignalR.ServerCheckMethods;
using SignalRLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Nodes;

namespace SignalRServ
{
    
    public class ChatHub : Hub<IHubMethods>
    {
        /// <summary>
        /// Отправка сообщений клиентам
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static UserContext db = new UserContext();
        [Authorize(Roles = "User,Admin,Anonymous")]
        public Task SendMessage(Message message)
        {
            if (!string.IsNullOrWhiteSpace(Context.Items["user_name"].ToString())
                && !string.IsNullOrWhiteSpace(Context.Items["user_group"].ToString()))
            {
                message.Sender = Context.Items["user_name"].ToString();
                message.DateSend = DateTime.Now;
                return Clients.OthersInGroup(Context.Items["user_group"].ToString()).Send(message);
            }
            return Task.CompletedTask;
        }
        [Authorize(Roles = "User,Admin,Anonymous")]
        public Task SendAudio(byte[] message)
        {
            if (!string.IsNullOrWhiteSpace(Context.Items["user_name"].ToString())
                && !string.IsNullOrWhiteSpace(Context.Items["user_group"].ToString()))
            {
                return Clients.OthersInGroup(Context.Items["user_group"].ToString()).SendAudio(Context.Items["user_name"].ToString(), message);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Установка имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Authorize(Roles = "User,Admin,Anonymous")]
        public string SetName(string name)
        {
            name = name.Trim();
            dynamic obj = JsonNode.Parse(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Startup.lastToken.Split('.')[1])));
            bool IsAuthorize = (string)obj["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] == "Anonymous" ? false : true;
            //bool IsRegistered = false;//ServerCheckMethods.CheckName(name).Result;//Нужно исправить
           

            if (!IsAuthorize)
            {
                int temp = db.Users.ToList().Where(a => a.UserName != null && a.UserName.SafeSubstring(0, a.UserName.IndexOf('(')).ToLower() == name.ToLower()).Count();
                name += $"({temp + 1})";
            }
                //if (IsAuthorize==false &&_auth==true)
                //{
                //    int temp = 1;

                //    temp = db.Users.ToList().Where(a =>a.UserName!=null && a.UserName.SafeSubstring(0, a.UserName.IndexOf('(')).ToLower() == name.ToLower()).Count();
                //    name += $"({temp + 1})";
                //}
                //if (IsAuthorize == false && _auth == false)
                //{
                //    int temp = db.Users.ToList().Where(a => a.UserName != null && a.UserName.SafeSubstring(0, a.UserName.IndexOf('(')).ToLower() == name).Count();
                //    if (temp >= 1) name += $"({temp + 1})";
                //}
                //else
                //{
                //    var userid = db.Users.ToList().Where(a => a.UserId == Context.ConnectionId).FirstOrDefault();
                //    if (userid != null) userid.UserName = name.Trim();
                //    Context.Items["user_name"] = name.Trim();
                //}
                //Context.Items["user_name"] = name.Trim();



                ////if(!IsAuthorize)
                ////    Context.Items["user_name"] = ServerCheckMethods.CheckName(name, IsAuthorize).Result;
            Context.Items["user_name"] = name;
            var userid = db.Users.ToList().Where(a => a.UserId == Context.ConnectionId).FirstOrDefault();
            if (userid != null) userid.UserName = name;
            Console.WriteLine($"++ {name} logged in {DateTime.Now}");
            return name;
        }
        /// <summary>
        /// Подключение к круппе
        /// </summary>
        /// <param name="group"></param>
        /// <returns>true удчаное подключение к группе</returns>
        [Authorize(Roles = "User,Admin,Anonymous")]
        public bool GroupConnect(string group)
        {
            if (!string.IsNullOrWhiteSpace(Context.Items["user_group"].ToString())) GroupDisconnect();
            //  var temp = db.Rooms.Select(a => a.Users.Select(b => b.UserId == Context.ConnectionId));
            // if (temp.Count() > 0) GroupDisconnect();

            var room = db.Rooms.ToList().Find(a => a.RoomName == group);
            if (room == null) return false;

            var user = db.Users.ToList().Where(a => a.UserId == Context.ConnectionId).FirstOrDefault();
            room.Users.Add(user);
            user._Room = room;
            //Context.Items.TryAdd("user_group", group);
            Context.Items["user_group"] = group;
            Groups.AddToGroupAsync(Context.ConnectionId, group);
            Clients.All.UpdateRooms();

            return true;
        }
        [Authorize(Roles = "User,Admin,Anonymous")]
        public Task GroupDisconnect()
        {
            var message = new Message
            {
                Text = $"{Context.Items["user_name"]} disconnected from {Context.Items["user_group"]} room",
                Sender = "Server",
                DateSend = DateTime.Now
            };
            Clients.OthersInGroup(Context.Items["user_group"].ToString()).Send(message);

            var user = db.Users.ToList().FirstOrDefault(u => u.UserId == Context.ConnectionId);
            if (user != null && user._Room!=null)
            {
                // удалить пользователя из комнаты
                RemoveFromRoom(user._Room.RoomName);
            }
            //Context.Items.TryAdd("user_group", string.Empty);
            Context.Items["user_group"] = string.Empty;
            Clients.All.UpdateRooms();
            return Task.CompletedTask;
        }
        [Authorize(Roles = "User,Admin,Anonymous")]
        public bool GroupCreate(string group)
        {
            var room = db.Rooms.Find(a => a.RoomName == group);
            if (room == null)
            {
                if (!string.IsNullOrWhiteSpace(Context.Items["user_group"].ToString())) GroupDisconnect();
                //  var temp = db.Rooms.Select(a => a.Users.Select(b => b.UserId == Context.ConnectionId));
                //    if (temp.Count() > 0) GroupDisconnect();
                ConversationRoom cr = new ConversationRoom()
                {
                    RoomName = group
                };

                db.Rooms.Add(cr);
                GroupConnect(group);
                //Context.Items.TryAdd("user_group", group);
                Context.Items["user_group"] = group;
                Console.WriteLine($"++ group {group} сreated {DateTime.Now}");
                return true;
            }
            return false;
        }
        [Authorize(Roles = "User,Admin,Anonymous")]
        public void RemoveFromRoom(string roomName)
        {
            //Узнать, существует ли комната
            var room = db.Rooms.ToList().Find(a => a.RoomName == roomName);

            if (room != null)
            {
                //Найдите пользователя для удаления
                var user = room.Users.ToList().Where(a => a.UserId == Context.ConnectionId).FirstOrDefault();
                //удалить этого пользователя
                room.Users.Remove(user);
                user._Connection = null;
                user._Room = null;
                //Если количество людей в комнате равно 0, удалить комнату
                if (room.Users.Count <= 0)
                {
                    db.Rooms.Remove(room);
                    Console.WriteLine($"-- group {roomName} deleted {DateTime.Now}");
                    Clients.All.UpdateRooms();
                }
                Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
                Clients.All.UpdateRooms();
            }
        }
        #region Event
        //События которые вызываются сами при:подключении, отключении, очистки ресурсов
        [Authorize(Roles = "User,Admin,Anonymous")]
        public override Task OnConnectedAsync()
        {
            Clients.All.UpdateRooms();
            Context.Items.TryAdd("user_name", string.Empty);
            Context.Items.TryAdd("user_group", string.Empty);

            var user = db.Users.ToList().SingleOrDefault(u => u.UserId == Context.ConnectionId);

            if (user == null)
            {
                user = new User()
                {
                    UserId = Context.ConnectionId
                };
                db.Users.Add(user);
            }

            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"-- {Context.Items["user_name"]} logged out {DateTime.Now}");
            var message = new Message
            {
                Text = $"{Context.Items["user_name"]} disconnected from {Context.Items["user_group"]} room",
                Sender = "Server",
                DateSend = DateTime.Now
            };
            Clients.OthersInGroup(Context.Items["user_group"].ToString()).Send(message);

            var user = db.Users.ToList().FirstOrDefault(u => u.UserId == Context.ConnectionId);
            if (user != null)
            {
                try
                {
                    if (user._Room != null)
                        RemoveFromRoom(user._Room.RoomName);
                    db.Users.Remove(user);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            Clients.All.UpdateRooms();
            return base.OnDisconnectedAsync(exception);
        }
        [Authorize(Roles = "User,Admin,Anonymous")]
        public List<string> GetRooms()
        {
            List<string> rooms = db.Rooms.ToList().Select(a => a.RoomName).ToList();
            return rooms;
        }
        [Authorize(Roles = "User,Admin,Anonymous")]
        public List<ServerUser> GetUsersRoom(string group)
        {
            //var itme = from a in db.Rooms
            //           where a.RoomName.ToString() == @group
            //           select a.Users.;
            //return itme.ToList<string>();
            var room = db.Rooms.ToList().Find(a => a.RoomName == group);
            if (room != null)
            {
                return room.Users.ToList().Select(a => new ServerUser(a.UserName)).ToList();
            }
            return new List<ServerUser>() { null };
        }
        [Authorize(Roles = "User,Admin,Anonymous")]
        public List<RoomsAndUsers> GetRoomsAndUsers()
        {
            List<RoomsAndUsers> temp = new List<RoomsAndUsers>();
            foreach (var a in db.Rooms)
                temp.Add(new RoomsAndUsers(a.RoomName, a.Users.ToList().Select(a => new ServerUser(a.UserName)).ToList()));
            return temp;
        }
        [Authorize(Roles = "User,Admin,Anonymous")]

        /// <summary>
        /// Выйти из чата
        /// </summary>
        /// <param name="roomName"></param>
        [Authorize(Roles = "User,Admin,Anonymous")]
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        #endregion
    }
}