using System;
namespace Netflix_backend.Models
{
    public class MovieModel
    {
        public String MovieId { get; set; }

        public String MovieUrl { get; set; }

        public String MovieThumbnail { get; set; }

        public String Title { get; set; }

        public String Description { get; set; }

        public float Imdb { get; set; }

        public String MovieRating { get; set; } // pg-13

        public String Year { get; set; }

        public String[] Genres { get; set; }

        public int Duration { get; set; }

        public float Rating { get; set; }

        public MovieModel()
        {
            this.MovieId = Guid.NewGuid().ToString("N");
        }

        public void updateRating(float new_rating) {
            this.Rating += new_rating;
            this.Rating /= 2;
        }

    }
}
