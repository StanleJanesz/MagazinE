﻿using MagazinEAPI.Models.Articles;
using MagazinEAPI.Models.Articles.Comment;

namespace MagazinEAPI.Models.Users.Readers
{
    public class User : Role
    {
        public List<Subscription> Subscriptions { get; set; } = []; //represents one-to-many User-Subscription
        public List<Ban> Bans { get; set; } = []; //represents one-to-many User-Ban
        public List<Article> ArticleFavoriteArticles { get; set; } = [];
        public List<FavoriteArticle> FavoriteArticles { get; set; } = []; //represents many-to-many User-Article
        public List<Article> ArticleToReadArticles { get; set; } = [];
        public List<ToReadArticle> ToReadArticles { get; set; } = [];//represents many-to-many User-Article
        public List<Tag> FavouriteTags { get; set; } = [];
        public List<TagUser> FavouriteTagUsers { get; set; } = []; //represents many-to-many User-Tag
        public List<Comment> Comments { get; set; } = []; // 1 to 0..n 
        public List<CommentReport> ReportedComments { get; set; } = []; //one to many
        public List<Comment> LikeComments { get; set; } = [];
        public List<Like> Likes { get; set; } = []; //represents many-to-many User-Comment
        public List<Comment> DislikeComments { get; set; } = [];
        public List<Dislike> Dislikes { get; set; } = []; //represents many-to-many User-Comment
    }
}
