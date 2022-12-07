using System;
namespace Netflix_backend.Models
{
    public class Genre
    {
        public String GenreId { get; set; }
        public String Name { get; set; }
        public Genre()
        {
        }
        public Genre(String Name) {
            this.GenreId = Guid.NewGuid().ToString("N");
            this.Name = Name;
        }

    }
}
