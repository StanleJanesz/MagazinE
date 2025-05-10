using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Base_Classes___Database
{
	public abstract class RegisterRequestAbstract
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }


		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

	}

}
