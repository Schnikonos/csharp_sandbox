namespace MyApp.Domain
{
    public class Book
    {
        public int Id { get; set; }
        public required string Title { get; set; } = null!;

        public int AuthorId { get; set; }
        public Author? Author { get; set; }

        public string? Description { get; set; }
    }
}
