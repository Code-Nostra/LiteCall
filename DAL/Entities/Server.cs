using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
	public class Server:IEntity
	{
		[Key]
		public int id { get; set; }
		[Required]
		public string Title { get; set; }
		public string Ip { get; set; }
		public string Ident { get; set; }
		public string Country { get; set; }
		public string City { get; set; }
		public string Description { get; set; }
	}
}
