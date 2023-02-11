﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
	public class SequrityQuestion
	{
		[Key]
		public int id { get; set; }

		[Required]
		public string Questions { get; set; }
	}
}
