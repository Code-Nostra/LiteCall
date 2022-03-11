using System.ComponentModel.DataAnnotations;

namespace AuthorizationServ
{
    public class UserDB
    {
        [Key]
        public int id { get; set; }
        public string Name { get; set; }

        public string Password { get; set; }
        public string Role { get; set; }
    }
}