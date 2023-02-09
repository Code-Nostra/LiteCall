using System.ComponentModel.DataAnnotations;

namespace ServerAuthorization.Models
{
    public class ServerDB
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Ip { get; set; }
        public string Description { get; set; }
        public string Ident { get; set; }
    }

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
        public int SecurityQuestionId { get; set; }
        public string AnswerSecurityQ { get; set; }
    }

    public class SecurityQuestions
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string Questions { get; set; }
    }
}