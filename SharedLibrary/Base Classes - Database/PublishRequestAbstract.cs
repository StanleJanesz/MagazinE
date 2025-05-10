using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Base_Classes___Database
{

	public abstract class PublishRequestAbstract
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public int ArticleId { get; set; }
		public PublishState PublishState { get; set; }
	}
}
