using System.ComponentModel.DataAnnotations;
using ServerAuthorization.Infrastructure;

namespace ServerAuthorization.Models.ViewModels
{
    public class ChangePassword
	{
		[Required]
		[StringLength(maximumLength: 15, MinimumLength = 4)]
		public string Login { get; set; }

		[Required]
		[StringLength(200, MinimumLength = 6)]
		private string _password;
		public string newPassword
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

		[Required(ErrorMessage = "Select security question")]
		[Range(1, 10)]
		public int QuestionsId { get; set; }

		[Required(ErrorMessage = "Enter the answer to the security question")]
		[StringLength(maximumLength: 100, MinimumLength = 5)]
		public string AnswersecurityQ { get; set; }
	}
}
