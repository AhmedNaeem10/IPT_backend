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
        public Genre(String GenreId, String Name) {
            this.GenreId = GenreId;
            this.Name = Name;
        }

    }
}
