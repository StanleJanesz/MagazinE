using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Base_Classes___Database
{
	public abstract class SubscriptionAbstract
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public SubscriptionState State { get; set; }
	}
}
