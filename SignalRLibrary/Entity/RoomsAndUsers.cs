﻿using System.Collections.Generic;

namespace SignalRLibrary
{
    public class RoomsAndUsers
    {
        public string RoomName { get; set; }
        public List<ServerUser> Users { get; set; }
       
        public bool Guard { get; set; }
        public RoomsAndUsers(string _RoomName, List<ServerUser> _Users,bool _guard)
        {
            RoomName = _RoomName;
            Users = _Users;
            Guard = _guard;
        }
    }
    
    public class ServerUser
    {
        public string Login { get; set; }

        public string Role { get; set; }

        public ServerUser(string _Login,string _Role)
        {
            Login = _Login;
            Role = _Role;
        }
    }
}