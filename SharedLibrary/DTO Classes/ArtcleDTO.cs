using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary.Base_Classes___Database;

namespace SharedLibrary.DTO_Classes
{
	public class ArticleDTO : ArticleAbstract
	{
		public List<int>? CommentsIds { get; set; }
		public List<string>? Photos { get; set; }
		public List<int>? TagsIds { get; set; }

	}
}
