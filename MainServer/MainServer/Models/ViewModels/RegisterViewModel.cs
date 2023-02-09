using System.ComponentModel.DataAnnotations;
using MainServer.Infrastructure;

namespace MainServer.Models.ViewModels
{
    public class RegisterViewModel
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
		[Range(0, 15)]
		public int QuestionsId { get; set; }
	}
}
