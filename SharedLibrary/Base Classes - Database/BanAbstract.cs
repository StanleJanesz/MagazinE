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
		public int UserId { get; set; }
		public int AdminId { get; set; }
		public string Reason { get; set; } 
		public bool Active { get; set; } //if ban is active or unbanned
		public DateTime BanStartDate { get; set; } //when user was banned
		public DateTime? BanEndDate { get; set;} //when user was unbanned (if was unbaned so nullable)
	}
}
