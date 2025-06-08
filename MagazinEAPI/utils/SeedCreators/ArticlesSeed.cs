using MagazinEAPI.Models.Articles;
using Microsoft.EntityFrameworkCore;

namespace MagazinEAPI.utils.SeedCreators
{
    public static class ArticlesSeed  // FOR TESTING PURPOSES ONLY
    {
        public static void InitializeArticles(ModelBuilder modelBuilder)
        {
            var articles = GenerateUniqueArticles();
            modelBuilder.Entity<Article>().HasData(articles);
        }

        private static List<Article> GenerateUniqueArticles()
        {
            var articles = new List<Article>();
            var baseDate = new DateTime(2024, 1, 1);

            var titles = new[]
            {
                "The Future of Technology",
                "Exploring Space: Mars Mission",
                "Climate Change and Its Impact",
                "The Rise of Electric Vehicles",
                "AI in Healthcare",
                "Blockchain Beyond Bitcoin",
                "The Psychology of Social Media",
                "History of Modern Art",
                "Cybersecurity Trends 2025",
                "The Evolution of Smartphones",
                "Gen Z in the Workforce",
                "Renewable Energy Innovations",
                "The Science Behind Sleep",
                "Sports Analytics: A New Era",
                "Understanding Quantum Computing",
                "Smart Cities: Living Tomorrow",
                "Mental Health Awareness",
                "The Growth of Remote Work",
                "Ocean Conservation Efforts",
                "Gaming Industry: What's Next?"
            };

            var intros = new[]
            {
                "Technology is evolving faster than ever.",
                "Mars might be our next home.",
                "Climate change is reshaping our world.",
                "Electric cars are changing the auto industry.",
                "AI is transforming how we diagnose illness.",
                "Blockchain isn’t just for crypto.",
                "Social media affects our mental state.",
                "Art has changed radically in the 20th century.",
                "Cyber threats are getting more complex.",
                "Phones have come a long way.",
                "Gen Z is redefining work culture.",
                "Green tech is our future.",
                "Sleep is more important than we think.",
                "Sports now run on data.",
                "Quantum computers could solve the unsolvable.",
                "Cities are getting smarter.",
                "Mental health should be a priority.",
                "Working from home is here to stay.",
                "Our oceans need help.",
                "Gaming is now bigger than Hollywood."
            };

            var contents = new[]
            {
                "Tech giants are investing in AI, quantum computing, and biotech.",
                "NASA and SpaceX plan new missions to explore Mars by 2030.",
                "Rising sea levels and extreme weather are the new normal.",
                "EVs are cleaner and smarter, with self-driving features.",
                "AI is helping doctors detect diseases earlier than ever.",
                "Supply chains and voting systems are being rebuilt with blockchain.",
                "Studies link screen time with anxiety and depression.",
                "From Picasso to Pollock, modern art broke all rules.",
                "Cybersecurity now relies on AI to detect anomalies.",
                "Smartphones have become digital Swiss Army knives.",
                "Flexible hours and values drive Gen Z’s work choices.",
                "Wind, solar, and storage are reshaping power grids.",
                "Sleep regulates memory, mood, and immune function.",
                "Data analysis improves performance and reduces injuries.",
                "Quantum tech could revolutionize encryption and materials science.",
                "Smart infrastructure optimizes traffic, energy, and safety.",
                "Schools and workplaces must support mental wellness.",
                "Remote tools like Zoom and Slack dominate business.",
                "Plastic pollution and warming threaten marine life.",
                "Games are cultural icons with massive economic impact."
            };

            for (int i = 0; i < 20; i++)
            {
                articles.Add(new Article
                {
                    Id = i + 1,
                    AuthorId = 1,
                    ReviewerId = 1,
                    isPremium = i % 2 == 0,
                    isPublished = true,
                    Title = titles[i],
                    Introduction = intros[i],
                    Content = contents[i],
                    TimeOfPublication = baseDate.AddDays(i)
                });
            }

            return articles;
        }
    }
}
