using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netflix_backend.Models
{
    public class MovieGet
    {
        public string MovieId { get; set; }

        public string TrailerUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        public string PosterUrl { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public float Imdb { get; set; }

        public string MovieRating { get; set; } // pg-13

        public string Year { get; set; }

        public List<string> Genres { get; set; } = new List<string>();

        public string Duration { get; set; }

        public float Rating { get; set; }

        public DateTime createdOn { get; set; } = DateTime.UtcNow;
    }
}