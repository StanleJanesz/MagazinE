using MagazinEAPI.Models.Articles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace MagazinEAPI.utils.SeedCreators
{
    public static class TagsSeed  // FOR TESTING PURPOSES ONLY
    {
        public static void InitializeTags(ModelBuilder modelBuilder)
        {
            var tags = GenerateUniqueTags();
            modelBuilder.Entity<Tag>().HasData(tags);
        }

        private static List<Tag> GenerateUniqueTags()
        {
            var tags = new List<Tag>();

            for (int i = 0; i < 20; i++)
            {
                tags.Add(new Tag
                {
                    Id = i + 1,
                    Name = $"Super tag {i + 1}"
                });
            }

            return tags;
        }
    }
}
