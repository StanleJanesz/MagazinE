using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DTO_Classes
{
	public class UserDTO : RoleDTO
	{
		public bool SubscriptionState { get; set; }
		public List<int> FavouriteArticlesIds { get; set; }
		public List<int> ArticlesToReadIds { get; set; }
		public List<int> FavouriteTagsIds { get; set; }
		public List<int> CommentsIds { get; set; }
		public List<int> LikedCommentsIds { get; set; }
		public List<int> UnlikedCommentsIds { get; set; }
	}
}
