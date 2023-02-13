using MainServer.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
	public class User:IEntity
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
}
