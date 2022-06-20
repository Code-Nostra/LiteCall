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
using System.Security.Claims;

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
        [Authorize(Roles = "User,Admin,Anonymous,Moderator")]
        public Task SendMessage(Message message)
        {
            if (!string.IsNullOrWhiteSpace(Context.Items["user_name"].ToString())
                && !string.IsNullOrWhiteSpace(Context.Items["user_group"].ToString()))
            {
                message.Sender = Context.Items["user_name"].ToString();
                message.DateSend = DateTime.Now;
                return  Clients.OthersInGroup(Context.Items["user_group"].ToString()).Send(message);
            }
            return Task.CompletedTask;
        }
        [Authorize(Roles = "User,Admin,Anonymous,Moderator")]
        public Task SendAudio(byte[] message)
        {
            if (!string.IsNullOrWhiteSpace(Context.Items["user_name"].ToString())
                && !string.IsNullOrWhiteSpace(Context.Items["user_group"].ToString()))
            {
                return Clients.OthersInGroup(Context.Items["user_group"].ToString())
                    .SendAudio(Context.Items["user_name"].ToString(), message);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Установка имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Authorize(Roles = "User,Admin,Anonymous,Moderator")]
        public string SetName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "Error";
            name = name.Trim();
            dynamic obj = JsonNode.Parse(Encoding.UTF8.GetString(WebEncoders.
                Base64UrlDecode(Startup.lastToken.Split('.')[1])));
            string role = (string)obj["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
            bool IsAuthorize = role 
                == "Anonymous" ? false : true;
            if (IsAuthorize)
            {
                var us = db.Users.Find(x => x.CName == name);
                if (us != null) return "non";
            }
            int temp = 0;
            if (!IsAuthorize)
            {
                var temp2 = db.Users.ToList().FindAll(x => x.CName?.ToLower() == name.ToLower());
                if (temp2 != null && temp2.Count!=0) temp=temp2.Max(x => x.Count);
                temp += 1;
            }
            
            #region legacy
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
            #endregion
            var userid = db.Users.ToList().Where(a => a.UserId == Context.ConnectionId).FirstOrDefault();
            if (userid != null)
            {
                userid.UserName = name;
                userid.Count = temp;
            }
            Context.Items["user_name"] = userid.UserName;
            userid.Role = role;
            Console.WriteLine($"++ {userid.UserName}  logged in {DateTime.Now}");
            return userid.UserName;
        }
        /// <summary>
        /// Подключение к круппе
        /// </summary>
        /// <param name="group"></param>
        /// <returns>true удчаное подключение к группе</returns>
        [Authorize(Roles = "User,Admin,Anonymous,Moderator")]
        public bool GroupConnect(string group,string password= "")
        {
            if (!string.IsNullOrWhiteSpace(Context.Items["user_group"].ToString())) GroupDisconnect();

            var room = db.Rooms.ToList().Find(a => a.RoomName == group);
            if ((room == null || room.Password != password) && !string.IsNullOrEmpty(password)) return false;

            var user = db.Users.ToList().Where(a => a.UserId == Context.ConnectionId).FirstOrDefault();
            room.Users.Add(user);
            user._Room = room;
            Context.Items["user_group"] = group;
            Groups.AddToGroupAsync(Context.ConnectionId, group);
            var message = new Message
            {
                Text = $"{Context.Items["user_name"]} connected from {Context.Items["user_group"]} room",
                Sender = "Server",
                DateSend = DateTime.Now
            };
            Clients.OthersInGroup(Context.Items["user_group"].ToString()).Send(message);
            Clients.All.UpdateRooms();

            return true;
        }
        [Authorize(Roles = "User,Admin,Anonymous,Moderator")]
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
            Context.Items["user_group"] = string.Empty;
            Clients.All.UpdateRooms();
            return Task.CompletedTask;
        }

        [Authorize(Roles = "Admin,Moderator")]
        public Task AdminKickUser(string _user)
        {
            var user = db.Users.ToList().FirstOrDefault(u => u.UserName == _user);
            if(string.IsNullOrEmpty(user._Room.RoomName)) return Task.CompletedTask;
            Message message = new Message
            {
                Text = $"{user.UserName} disconnected from {user._Room.RoomName} room",
                Sender = "Server",
                DateSend = DateTime.Now
            };
            if(string.IsNullOrEmpty(user._Room.RoomName)) return Task.CompletedTask;
            Clients.OthersInGroup(user._Room.RoomName?.ToString()).Send(message);
            bool kick = true;
            Clients.Client(user.UserId).Notification(kick);

            if (user != null && user._Room != null)
            {
                // удалить пользователя из комнаты
                KickFromRoom(user._Room.RoomName, user.UserId);
                user.UserContext.Items["user_group"] = string.Empty;
            }
            
            Clients.All.UpdateRooms();
            
            //await Clients.Client(user.UserId).Send("ReceiveMessageToUser", user, targetConnectionId, message);
            return Task.CompletedTask;
        }
        [Authorize(Roles = "User,Admin,Anonymous,Moderator")]
        public bool GroupCreate(string group,string password="")
        {
            var room = db.Rooms.Find(a => a.RoomName.ToLower().Trim() == group.ToLower().Trim());
            if (room == null)
            {
                if (!string.IsNullOrWhiteSpace(Context.Items["user_group"].ToString())) GroupDisconnect();

                ConversationRoom cr = new ConversationRoom()
                {
                    RoomName = group.Trim(),
                    Password=password
                };
                
                db.Rooms.Add(cr);
                GroupConnect(group,password);
                Context.Items["user_group"] = group;
                Console.WriteLine($"++ group {group} created {DateTime.Now}");
                return true;
            }
            return false;
        }

        //[Authorize(Roles = "Admin")]
        //public bool StandingGroupCreate(string group, string password = null)
        //{
        //    var room = db.Rooms.Find(a => a.RoomName == group);
        //    if (room == null)
        //    {
        //        if (!string.IsNullOrWhiteSpace(Context.Items["user_group"].ToString())) GroupDisconnect();

        //        ConversationRoom cr = new ConversationRoom()
        //        {
        //            RoomName = group,
        //            Password = password
        //        };

        //        db.Rooms.Add(cr);
        //        GroupConnect(group);
        //        Context.Items["user_group"] = group;
        //        Console.WriteLine($"++ group {group} сreated {DateTime.Now}");
        //        return true;
        //    }
        //    return false;
        //}
        [Authorize(Roles = "Admin,Moderator")]
        public void RemoveFromRoom(string roomName)
        {
            //Узнать, существует ли комната
            var room = db.Rooms.ToList().Find(a => a.RoomName == roomName);

            if (room != null)
            {
                //Найдите пользователя для удаления
                var user = room.Users.ToList().Where(a => a.UserId == Context.ConnectionId).FirstOrDefault();
                //удалить этого пользователя из конкретной комнаты
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
        [Authorize(Roles = "Admin,Moderator")]
        public void AdminDeleteRoom(string roomName)
        {
            //Узнать, существует ли комната
            var room = db.Rooms.ToList().Find(a => a.RoomName == roomName);

            if (room != null)
            {
                Clients.Group(roomName).Notification(true);
                foreach (var us in room.Users)
                {
                    us.UserContext.Items["user_group"] = string.Empty;
                    us._Connection = null;
                    us._Room = null;
                    Groups.RemoveFromGroupAsync(us.UserId, roomName);
                }
                
                db.Rooms.Remove(room);
                
                Console.WriteLine($"-- group {roomName} deleted {DateTime.Now}");
                Clients.All.UpdateRooms();
            }
        }


        [Authorize(Roles = "Admin,Moderator")]
        public void KickFromRoom(string roomName,string _userid)
        {
            //Узнать, существует ли комната
            var room = db.Rooms.ToList().Find(a => a.RoomName == roomName);

            if (room != null)
            {
                //Найдите пользователя для удаления
                var user = room.Users.ToList().Where(a => a.UserId == _userid).FirstOrDefault();
                //удалить этого пользователя из конкретной комнаты
                room.Users.Remove(user);
                user._Connection = null;
                user._Room = null;
                Groups.RemoveFromGroupAsync(user.UserId, roomName);
                //Если количество людей в комнате равно 0, удалить комнату
                if (room.Users.Count <= 0)
                {
                    db.Rooms.Remove(room);
                    Console.WriteLine($"-- group {roomName} deleted {DateTime.Now}");
                    Clients.All.UpdateRooms();
                }
                
                Clients.All.UpdateRooms();
            }
        }

        #region Event
        //События которые вызываются сами при:подключении, отключении, очистки ресурсов
        [Authorize(Roles = "User,Admin,Anonymous,Moderator")]
        public override Task OnConnectedAsync()
        {
            Clients.All.UpdateRooms();
            Context.Items.TryAdd("user_name", string.Empty);
            Context.Items.TryAdd("user_group", string.Empty);
           
            var user = db.Users.SingleOrDefault(u => u.UserId == Context.ConnectionId);
            
            if (user == null)
            {
                user = new User()
                {
                    UserId = Context.ConnectionId,
                    UserContext = Context
                };
                db.Users.Add(user);
                
            }

            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"-- {Context.Items["user_name"]} logged out {DateTime.Now}");
            GroupDisconnect();
            var user = db.Users.ToList().FirstOrDefault(u => u.UserId == Context.ConnectionId);
            if (user != null)
            {
                db.Users.Remove(user);
            }
            Clients.All.UpdateRooms();
            return base.OnDisconnectedAsync(exception);
        }
        [Authorize(Roles = "User,Admin,Anonymous,Moderator")]
        public Dictionary<string, bool> GetRooms()
        {
            Dictionary<string,bool> rooms = db.Rooms.ToList().ToDictionary(x=>x.RoomName,x=>x.Guard);
            return rooms;
        }
        [Authorize(Roles = "User,Admin,Anonymous,Moderator")]
        public List<ServerUser> GetUsersRoom(string group)
        {
            var room = db.Rooms.ToList().Find(a => a.RoomName == group);
            if (room != null)
            {
                return room.Users.ToList().Select(a => new ServerUser(a.UserName,a.Role)).ToList();
            }
            return new List<ServerUser>() { null };
        }
        [Authorize(Roles = "User,Admin,Anonymous,Moderator")]
        public List<RoomsAndUsers> GetRoomsAndUsers()
        {
            List<RoomsAndUsers> temp = new List<RoomsAndUsers>();
            foreach (var a in db.Rooms)
                temp.Add(new RoomsAndUsers(a.RoomName, a.Users.ToList().Select(a => 
                new ServerUser(a.UserName,a.Role)).ToList(),a.Guard));
            return temp;
        }

        
        [Authorize(Roles = "User,Admin,Anonymous,Moderator")]
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        #endregion
        
    }
}