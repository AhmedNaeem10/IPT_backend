using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Netflix_backend.Models
{
    public class MovieModel
    {
        public string MovieId { get; set; } = Guid.NewGuid().ToString("N");

        public IFormFile TrailerFile { get; set; }

        public IFormFile ThumbnailFile { get; set; }

        public IFormFile PosterFile { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public float Imdb { get; set; }

        public string AgeRating { get; set; } // pg-13

        public string Year { get; set; }

        public List<string> Genre { get; set; } = new List<string>();
        public string Duration { get; set; }

        public float Rating { get; set; }

        public void updateRating(float new_rating) {
            this.Rating += new_rating;
            this.Rating /= 2;
        }

    }
}
