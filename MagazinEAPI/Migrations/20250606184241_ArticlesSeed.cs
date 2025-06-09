using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagazinEAPI.Migrations
{
    /// <inheritdoc />
    public partial class ArticlesSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Articles",
                columns: new[] { "Id", "AuthorId", "Content", "Introduction", "ReviewerId", "TimeOfPublication", "Title", "isPremium", "isPublished" },
                values: new object[,]
                {
                    { 1, 1, "Tech giants are investing in AI, quantum computing, and biotech.", "Technology is evolving faster than ever.", 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Future of Technology", true, true },
                    { 2, 1, "NASA and SpaceX plan new missions to explore Mars by 2030.", "Mars might be our next home.", 1, new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Exploring Space: Mars Mission", false, true },
                    { 3, 1, "Rising sea levels and extreme weather are the new normal.", "Climate change is reshaping our world.", 1, new DateTime(2024, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Climate Change and Its Impact", true, true },
                    { 4, 1, "EVs are cleaner and smarter, with self-driving features.", "Electric cars are changing the auto industry.", 1, new DateTime(2024, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Rise of Electric Vehicles", false, true },
                    { 5, 1, "AI is helping doctors detect diseases earlier than ever.", "AI is transforming how we diagnose illness.", 1, new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "AI in Healthcare", true, true },
                    { 6, 1, "Supply chains and voting systems are being rebuilt with blockchain.", "Blockchain isn’t just for crypto.", 1, new DateTime(2024, 1, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Blockchain Beyond Bitcoin", false, true },
                    { 7, 1, "Studies link screen time with anxiety and depression.", "Social media affects our mental state.", 1, new DateTime(2024, 1, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Psychology of Social Media", true, true },
                    { 8, 1, "From Picasso to Pollock, modern art broke all rules.", "Art has changed radically in the 20th century.", 1, new DateTime(2024, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "History of Modern Art", false, true },
                    { 9, 1, "Cybersecurity now relies on AI to detect anomalies.", "Cyber threats are getting more complex.", 1, new DateTime(2024, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cybersecurity Trends 2025", true, true },
                    { 10, 1, "Smartphones have become digital Swiss Army knives.", "Phones have come a long way.", 1, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Evolution of Smartphones", false, true },
                    { 11, 1, "Flexible hours and values drive Gen Z’s work choices.", "Gen Z is redefining work culture.", 1, new DateTime(2024, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gen Z in the Workforce", true, true },
                    { 12, 1, "Wind, solar, and storage are reshaping power grids.", "Green tech is our future.", 1, new DateTime(2024, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Renewable Energy Innovations", false, true },
                    { 13, 1, "Sleep regulates memory, mood, and immune function.", "Sleep is more important than we think.", 1, new DateTime(2024, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Science Behind Sleep", true, true },
                    { 14, 1, "Data analysis improves performance and reduces injuries.", "Sports now run on data.", 1, new DateTime(2024, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sports Analytics: A New Era", false, true },
                    { 15, 1, "Quantum tech could revolutionize encryption and materials science.", "Quantum computers could solve the unsolvable.", 1, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Understanding Quantum Computing", true, true },
                    { 16, 1, "Smart infrastructure optimizes traffic, energy, and safety.", "Cities are getting smarter.", 1, new DateTime(2024, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Smart Cities: Living Tomorrow", false, true },
                    { 17, 1, "Schools and workplaces must support mental wellness.", "Mental health should be a priority.", 1, new DateTime(2024, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mental Health Awareness", true, true },
                    { 18, 1, "Remote tools like Zoom and Slack dominate business.", "Working from home is here to stay.", 1, new DateTime(2024, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Growth of Remote Work", false, true },
                    { 19, 1, "Plastic pollution and warming threaten marine life.", "Our oceans need help.", 1, new DateTime(2024, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ocean Conservation Efforts", true, true },
                    { 20, 1, "Games are cultural icons with massive economic impact.", "Gaming is now bigger than Hollywood.", 1, new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gaming Industry: What's Next?", false, true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 20);
        }
    }
}
