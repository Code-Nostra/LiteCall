using System.ComponentModel.DataAnnotations;

namespace AuthorizationServ
{
    public class UserDB
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
    }
}