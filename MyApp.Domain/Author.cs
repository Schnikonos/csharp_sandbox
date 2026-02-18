using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MyApp.Domain
{
    public class Author
    {
        public int Id { get; set; } = default!;
        public required string Name { get; set; } = null!;
        public required string? Surname { get; set; } = default!;

        [JsonIgnore]
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
