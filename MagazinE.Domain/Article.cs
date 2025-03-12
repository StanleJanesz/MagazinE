namespace MagazinE.Domain
{
    public class Article
    {
        int Id { get; set; }
        public bool IsPublished { get; private set; }
        public bool IsPremium { get; private set; }
        public int AuthorId { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public List<int> CommentIds { get; private set; }
        public List<string> Photos { get; private set; }
        public List<string> Tags { get; private set; }
        

        public Article(string title, int authorId) // TODO: decide if 
        {
            Title = title;
            AuthorId = authorId;
            CommentIds = new List<int>();
            Tags = new List<string>();
            Photos = new List<string>();
            IsPublished = false;
            IsPremium = false;
            Content = "";
        }

        public void AddComment(int commentId)
        {
            CommentIds.Add(commentId);
        }

        public void AddPhoto(string adres) // TODO: decide if we are using blob storage or not
        {
            Photos.Add(adres);
        }

        public void AddTag(string tag)
        {
            Tags.Add(tag);
        }

        public void Publish()
        {
            IsPublished = true;
        }

        public void MakePremium()
        {
            IsPremium = true;
        }

        public void ChangeContent(string content)
        {
            Content = content;
        }

        public void ChangeTitle(string title)
        {
            Title = title;
        }

        public bool removeTag(string tag)
        {
            return Tags.Remove(tag);
        }

        public bool removePhoto(string photo)
        {
            return Photos.Remove(photo);
        }

        public bool removeComment(int commentId)
        {
            return CommentIds.Remove(commentId);
        }

        public void Unpublish()
        {
            IsPublished = false;
        }

        public void MakeNotPremium()
        {
            IsPremium = false;
        }
    }
}
