using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DTO_Classes
{
	public class HeadEditorDTO : RoleDTO
	{
		public List<int> ArticlesIds { get; set; }
		public List<int> EditorsUnderIds { get; set; }
		public List<int> JournalistsUnderIds { get; set; }
	}
}
