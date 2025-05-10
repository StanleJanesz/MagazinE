using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Base_Classes___Database
{
	public abstract class CommentAbstract
	{
		public int Id { get; set; }
		public int AuthorId { get; set; }
		public string Content { get; set; }
		public DateTime Date { get; set; }
	}
}
