using System.ComponentModel.DataAnnotations;

namespace AuthorizationServ.Token
{
    public class AuthModel
    {
        [Required]
        [StringLength(maximumLength: 20, MinimumLength = 4)]
        public string Login { get; set; }
        [StringLength(maximumLength: 50, MinimumLength = 0)]
        public string Password { get; set; }

        [StringLength(maximumLength: 100, MinimumLength = 10)]
        public string Guid { get; set; }

        [StringLength(maximumLength: 6, MinimumLength = 4)]
        public string Captcha { get; set; }
    }
}