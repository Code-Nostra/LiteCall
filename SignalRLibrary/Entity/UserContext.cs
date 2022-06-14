using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRLibrary
{
    public class UserContext
    {
        public UserContext()
        {
            Users = new List<User>();
            Connections = new List<Connection>();
            Rooms = new List<ConversationRoom>();
        }
        /// <summary>
        /// Коллекция пользователей
        /// </summary>
        public List<User> Users { get; set; }

        /// <summary>
        /// Коллекция соединений
        /// </summary>
        public List<Connection> Connections { get; set; }

        /// <summary>
        /// Коллекция комнат
        /// </summary>
        public List<ConversationRoom> Rooms { get; set; }
    }

    public class User
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Key]
        public string UserId { get; set; }

        private string _username;
        public string UserName
        {
            get
            {
                return Count != 0 ? (_username + "(" + Count + ")") : _username;
            }
            set { _username = value; }
        }

        public string CName
        {
            get { return _username; }
        }

        public HubCallerContext UserContext {get;set;}

        /// <summary>
        /// Подключённые пользователи
        /// </summary>
        #region Чтобы один пользовател состоял в множестве чатов
        //public List<Connection> Connections { get; set; }

        ///// <summary>
        ///// Коллекция пользовательских комнат
        ///// </summary>
        //public virtual List<ConversationRoom> Rooms { get; set; }

        //public User()
        //{
        //    Connections = new List<Connection>();
        //    Rooms = new List<ConversationRoom>();
        //}
        #endregion
        
        public int Count { get; set; }
        ///Чтобы один пользоватль состоял в одной комнате
        public Connection _Connection { get; set; }

        /// <summary>
        ///  Комната
        /// </summary>
        public virtual ConversationRoom _Room { get; set; }

        public User()
        {
            _Connection = new Connection();
            _Room = new ConversationRoom();
        }

    }

    public class Connection
    {
        /// <summary>
        /// Идентификатор соединения
        /// </summary>
        public string ConnectionID { get; set; }

        /// <summary>
        /// Пользовательский агент
        /// </summary>
        //public string UserAgent { get; set; }

        /// <summary>
        /// Состояние подключения
        /// </summary>
        public bool Connected { get; set; }
    }

    /// <summary>
    /// Храним информацию о всех комнатах на сервере и списку пользователей в них
    /// </summary>
    public class ConversationRoom
    {
        /// <summary>
        /// Имя комнаты
        /// </summary>
        [Key]
        public string RoomName { get; set; }

        public string Password { get; set; }

        public bool Guard
        {
            get
            {
                return Password == string.Empty || Password == null ? false : true;
            }
        }
        /// <summary>
        /// Список пользователей в комнатах
        /// </summary>
        public virtual List<User> Users { get; set; }

        public ConversationRoom()
        {
            Users = new List<User>();
        }
    }
}
