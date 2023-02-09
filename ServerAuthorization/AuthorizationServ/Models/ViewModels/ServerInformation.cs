using ServerAuthorization.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServerAuthorization.Models.ViewModels
{
	public class ServerInformation : IValidatableObject
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

		[StringLength(maximumLength: 30, MinimumLength = 0, ErrorMessage = "Title can be maximum 16 characters")]
		public string Title { get; set; }

		[StringLength(maximumLength: 25, MinimumLength = 0, ErrorMessage = "IP can be maximum 25 characters")]
		public string Ip { get; set; }

		public string Ident { get; set; }


		[StringLength(maximumLength: 16, MinimumLength = 0, ErrorMessage = "Description can be maximum 16 characters")]
		public string Country { get; set; }
		[StringLength(maximumLength: 70, MinimumLength = 0, ErrorMessage = "Country can be maximum 70 characters")]
		public string City { get; set; }
		
		[StringLength(maximumLength: 16, MinimumLength = 0, ErrorMessage = "Description can be maximum 16 characters")]
		public string Description { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			List<ValidationResult> errors = new List<ValidationResult>();

			if (string.IsNullOrWhiteSpace(this.Title))
			{
				errors.Add(new ValidationResult("Введите название сервера!"));
			}
			if (string.IsNullOrWhiteSpace(this.Ip))
			{
				errors.Add(new ValidationResult("Введите IP адрес!"));
			}
			if (string.IsNullOrWhiteSpace(this.Ident))
			{
				errors.Add(new ValidationResult("Неверный идентификатор!"));
			}

			return errors;
		}
	}
}
