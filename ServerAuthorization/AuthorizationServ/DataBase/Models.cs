namespace AuthorizationServ.DataBase
{
    public class StandingRoomModel
    {
        public string Title { get; set; }
        public string Password { get; set; }
    }

    public class NameAndBool
    {
        public string Name { get; set; }
        public bool IsAuthorize { get; set; }
    }
}