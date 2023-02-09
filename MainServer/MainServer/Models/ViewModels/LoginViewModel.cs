using System.ComponentModel.DataAnnotations;
using MainServer.Infrastructure;

namespace MainServer.Models.ViewModels
{
    public class LoginViewModel
	{
		[Required]
		[StringLength(maximumLength: 15, MinimumLength = 4)]
		public string Login { get; set; }

		[StringLength(maximumLength: 200, MinimumLength = 0)]
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
}
