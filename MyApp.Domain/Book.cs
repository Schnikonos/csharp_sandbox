namespace MyApp.Domain
{
    public class Book
    {
        public int Id { get; set; }
        public required string Title { get; set; } = null!;

        public int AuthorId { get; set; }
        public required Author Author { get; set; } = null!;

        public required string? Description { get; set; }
    }
}
