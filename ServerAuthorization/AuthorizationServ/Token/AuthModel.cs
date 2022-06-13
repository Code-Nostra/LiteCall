using System.ComponentModel.DataAnnotations;

namespace AuthorizationServ.Token
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

        [StringLength(int.MaxValue,MinimumLength = 6 )]
        [Required]
        public string Password { get; set; }
        
        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 10)]
        public string Guid { get; set; }
        
        [Required]
        [StringLength(maximumLength: 5, MinimumLength = 4)]
        public string Captcha { get; set; }
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
        [StringLength(maximumLength: 100, MinimumLength = 6)]
        public string Questions { get; set; }
        
        [Required(ErrorMessage = "Enter the answer to the security question")]
        [StringLength(maximumLength: 100, MinimumLength = 5)]
        public string AnswersecurityQ { get; set; }
    }

}