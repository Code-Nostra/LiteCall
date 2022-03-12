namespace AuthorizationServ.DataBase
{
    public class InfoServerModel
    {
        public string Title { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Description { get; internal set; }
        public string Ip { get; internal set; }
    }

    public class NameAndBool
    {
        public string Name { get; set; }
        public bool IsAuthorize { get; set; }
    }
}