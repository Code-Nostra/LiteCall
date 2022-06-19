using System.ComponentModel.DataAnnotations;

namespace MainServer.Token
{
    public class AuthModel
    {
        [Required]
        [StringLength(maximumLength: 15, MinimumLength = 4)]
        public string Login { get; set; }
        [StringLength(int.MaxValue, MinimumLength = 0)]
        private string _password;
        public string Password
        {
            get
            {
                return _password.GetSha1();
            }
            set
            {
                _password = value;
            }
        }

    }

    public class RegModel
    {
        [Required]
        [StringLength(maximumLength: 15, MinimumLength = 4)]
        public string Login { get; set; }

        [StringLength(int.MaxValue, MinimumLength = 6)]
        [Required]
        public string Password { get; set; }

        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 10)]
        public string Guid { get; set; }

        [Required]
        [StringLength(maximumLength: 5, MinimumLength = 4)]
        public string Captcha { get; set; }

        [Required(ErrorMessage = "Enter the answer to the security question")]
        [StringLength(maximumLength: 100, MinimumLength = 5)]
        public string AnswersecurityQ { get; set; }

        [Required(ErrorMessage = "Select security question")]
        [Range(5, 100)]
        public int QuestionsId { get; set; }
    }

    public class ChangPassModel
    {
        [Required]
        [StringLength(maximumLength: 15, MinimumLength = 4)]
        public string Login { get; set; }

        [StringLength(int.MaxValue, MinimumLength = 6)]
        [Required]
        public string newPassword { get; set; }

        [Required(ErrorMessage = "Select security question")]
        [Range(5, 100)]
        public int QuestionsId { get; set; }

        [Required(ErrorMessage = "Enter the answer to the security question")]
        [StringLength(maximumLength: 100, MinimumLength = 5)]
        public string AnswersecurityQ { get; set; }
    }

    public class AddRole
    {
        [Required]
        [StringLength(maximumLength: 15, MinimumLength = 4)]
        public string Login { get; set; }
        [StringLength(int.MaxValue, MinimumLength = 0)]
        private string _password;
        public string Password
        {
            get
            {
                return _password.GetSha1();
            }
            set
            {
                _password = value;
            }
        }

        public string Role { get; set; }
        public string OpLogin { get; set; }
    }

    public class ServerInfo
    {
        [Required]
        [StringLength(maximumLength: 15, MinimumLength = 4)]
        public string Login { get; set; }
        [StringLength(int.MaxValue, MinimumLength = 0)]
        private string _password;
        public string Password
        {
            get
            {
                return _password.GetSha1();
            }
            set
            {
                _password = value;
            }
        }
        [StringLength(maximumLength: 30, MinimumLength = 0)]
        public string Title { get; set; }
        [StringLength(maximumLength: 40, MinimumLength = 0)]
        public string Country { get; set; }
        [StringLength(maximumLength: 430, MinimumLength = 0)]
        public string City { get; set; }
        [StringLength(maximumLength: 16, MinimumLength = 0)]
        public string Ip { get; set; }
        public string Description { get; set; }
    }
}