using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DTO_Classes
{
	public class EditorDTO : RoleDTO
	{
		public List<int> ArticlesIds { get; set; }
		public List<int> AllowedTagsIds { get; set; }
	}
}
