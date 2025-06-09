using MagazinEAPI.Models.Articles.Comment;
using Microsoft.EntityFrameworkCore;
using System;

namespace MagazinEAPI.utils.SeedCreators
{
    public static class CommentsSeed  // FOR TESTING PURPOSES ONLY
    {
        public static void InitializeComments(ModelBuilder modelBuilder)
        {
            var comments = GenerateUniqueComments();
            modelBuilder.Entity<Comment>().HasData(comments);
        }

        private static List<Comment> GenerateUniqueComments()
        {
            var comments = new List<Comment>();
            var baseDate = new DateTime(2024, 1, 1);

            for (int i = 0; i < 20; i++)
            {
                comments.Add(new Comment
                {
                    ArticleId = i + 1,
                    Id = i + 1,
                    AuthorId = 1,
                    Content = $"This is comment #{i + 1} on article #{i + 1}. What a great article!!!",
                });
            }

            return comments;
        }
    }
}
