﻿using System.ComponentModel.DataAnnotations;

namespace RazorPage_Web.Models
{
	public class User
	{
		[Key]
		public int Id { get; set; }


		public string FullName { get; set; }

		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string Password { get; set; }

	}
}
