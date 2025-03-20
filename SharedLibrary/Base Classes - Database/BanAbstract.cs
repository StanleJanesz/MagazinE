using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Base_Classes___Database
{
	public abstract class BanAbstract
	{
		public int Id { get; set; }
		public int AdminId { get; set; }
		public int UserId { get; set; }
		public string Reason { get; set; }
	}
}
