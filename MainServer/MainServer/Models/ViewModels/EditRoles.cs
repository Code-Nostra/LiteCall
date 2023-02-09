using System.ComponentModel.DataAnnotations;
using MainServer.Infrastructure;

namespace MainServer.Models.ViewModels
{
    public class EditRoles
	{
		[Required]
		[StringLength(maximumLength: 15, MinimumLength = 4)]
		public string Login { get; set; }
		[Required]
		[StringLength(maximumLength: 200, MinimumLength = 6)]
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
		public string roleTaker { get; set; }
	}
}
