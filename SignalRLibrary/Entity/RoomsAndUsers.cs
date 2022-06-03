using System.Collections.Generic;

namespace SignalRLibrary
{
    public class RoomsAndUsers
    {
        public string RoomName { get; set; }
        public List<ServerUser> Users { get; set; }
       
        public bool Guard { get; set; }
        public RoomsAndUsers(string _RoomName, List<ServerUser> _Users)
        {
            RoomName = _RoomName;
            Users = _Users;
        }
    }

    public class ServerUser
    {
        public string Login { get; set; }

        public ServerUser(string _Login)
        {
            Login = _Login;
        }
    }
}