using Microsoft.VisualStudio.TestTools.UnitTesting;
using MagazinE.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagazinE.Domain.Tests
{
    [TestClass()]
    public class ArticleTests
    {
        [TestMethod()]
        public void AddCommentTest()
        {
            Article article = new Article("Title", 1);
            article.AddComment(1);
            Assert.AreEqual(1, article.CommentIds.Count); 
            Assert.IsTrue( article.CommentIds.Exists(x => x == 1));
        }

        [TestMethod()]
        public void AddPhotoTest()
        {
            Article article = new Article("Title", 1);
            article.AddPhoto("photo");
            Assert.AreEqual(1, article.Photos.Count);
            Assert.IsTrue(article.Photos.Exists(x => x == "photo"));
        }

        [TestMethod()]
        public void AddTagTest()
        {
            Article article = new Article("Title", 1);
            article.AddTag("tag");
            Assert.AreEqual(1, article.Tags.Count);
            Assert.IsTrue(article.Tags.Exists(x => x == "tag"));
        }

        [TestMethod()]
        public void PublishTest()
        {
            Article article = new Article("Title", 1);
            article.Publish();
            Assert.IsTrue(article.IsPublished);
        }

        [TestMethod()]
        public void MakePremiumTest()
        {
            Article article = new Article("Title", 1);
            article.MakePremium();
            Assert.IsTrue(article.IsPremium);
        }

        [TestMethod()]
        public void ChangeContentTest()
        {
            Article article = new Article("Title", 1);
            article.ChangeContent("content");
            Assert.AreEqual("content", article.Content);
        }

        [TestMethod()]
        public void ChangeTitleTest()
        {
            Article article = new Article("Title", 1);
            article.ChangeTitle("new title");
            Assert.AreEqual("new title", article.Title);
        }

        [TestMethod()]
        public void removeTagTest()
        {
            Article article = new Article("Title", 1);
            article.AddTag("tag");
            article.AddTag("tag2");
            article.removeTag("tag");
            Assert.AreEqual(1, article.Tags.Count);
            Assert.IsFalse(article.Tags.Exists(x => x == "tag"));
            Assert.IsTrue(article.Tags.Exists(x => x == "tag2"));
        }

        [TestMethod()]
        public void removePhotoTest()
        {
            Article article = new Article("Title", 1);
            article.AddPhoto("photo");
            article.AddPhoto("photo2");
            article.removePhoto("photo");
            Assert.AreEqual(1, article.Photos.Count);
            Assert.IsFalse(article.Photos.Exists(x => x == "photo"));
            Assert.IsTrue(article.Photos.Exists(x => x == "photo2"));
        }

        [TestMethod()]
        public void removeCommentTest()
        {
            Article article = new Article("Title", 1);
            article.AddComment(1);
            article.AddComment(2);
            article.removeComment(1);
            Assert.AreEqual(1, article.CommentIds.Count);
            Assert.IsFalse(article.CommentIds.Exists(x => x == 1));
            Assert.IsTrue(article.CommentIds.Exists(x => x == 2));
        }

        [TestMethod()]
        public void UnpublishTest()
        {
            Article article = new Article("Title", 1);
            article.Publish();
            article.Unpublish();
            Assert.IsFalse(article.IsPublished);
        }

        [TestMethod()]
        public void MakeNotPremiumTest()
        {
            Article article = new Article("Title", 1);
            article.MakePremium();
            article.MakeNotPremium();
            Assert.IsFalse(article.IsPremium);
        }
    }
}