using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MainServer.Models.ViewModels
{
	public class ServerInformation : IValidatableObject
	{
		[StringLength(maximumLength: 30, MinimumLength = 0, ErrorMessage = "Title can be maximum 16 characters")]
		public string Title { get; set; }

		[StringLength(maximumLength: 25, MinimumLength = 0, ErrorMessage = "IP can be maximum 25 characters")]
		public string Ip { get; set; }

		public string Ident { get; set; }

		public string Country { get; set; }
		public string City { get; set; }

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
			if (string.IsNullOrWhiteSpace(this.Country))
			{
				errors.Add(new ValidationResult("Страна не задана!"));
			}
			if (string.IsNullOrWhiteSpace(this.City))
			{
				errors.Add(new ValidationResult("Город не задан!"));
			}
			if (string.IsNullOrWhiteSpace(this.Description))
			{
				errors.Add(new ValidationResult("Описание не задано!"));
			}
			return errors;
		}
	}
}
