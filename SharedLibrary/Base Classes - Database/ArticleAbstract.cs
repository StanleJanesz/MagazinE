using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Base_Classes___Database
{
	public abstract class ArticleAbstract
	{
		public int Id { get; set; }
		public bool isPremium { get; set; }
		public bool isPublished { get; set; }
		public string Title { get; set; }
		public int AuthorId { get; set; }
		public string Introduction { get; set; }
		public string Content { get; set; }
		public DateTime? TimeOfPublication { get; set; }
	}
}
