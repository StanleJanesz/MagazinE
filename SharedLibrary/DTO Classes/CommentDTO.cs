using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary.Base_Classes___Database;

namespace SharedLibrary.DTO_Classes
{
	public class CommentDTO : CommentAbstract
	{
		public int ParentId { get; set; }
		public List<int> ChildrenIds { get; set; }

		public int LikesCount { get; set; }
		public int DislikesCount { get; set; }
	}
}
