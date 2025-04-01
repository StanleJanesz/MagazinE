﻿using MagazinEAPI.Models.Users.Readers;

namespace MagazinEAPI.Models.Articles.Comment
{
    public class Dislike
    {
        public int UserId { get; set; }
        public int CommentId { get; set; }

        public User User { get; set; } = null!;
        public Comment Comment { get; set; } = null!;
    }
}
