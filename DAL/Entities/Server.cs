using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
	public class Server
	{
		[Key]
		public int id { get; set; }
		[Required]
		public string Title { get; set; }
		public string Ip { get; set; }
		public string Ident { get; set; }
	}
}
