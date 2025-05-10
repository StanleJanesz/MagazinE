using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DTO_Classes
{
	public class AdminDTO : RoleDTO
	{
		public List<int> BannedUsersIds { get; set; }
		public List<int> UnbanRequestsIds { get; set; }
		public List<int> DeletedCommentsIds { get; set; }
	}
}
