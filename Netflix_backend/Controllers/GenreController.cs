using System;
using System.Net.Http;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Config;
using Microsoft.AspNetCore.Mvc;
using FireSharp.Interfaces;
using FireSharp;
using Netflix_backend.Models;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Netflix_backend.Controllers
{
    public class GenreController
    {
        public GenreController()
        {
        }

        public static void Add(String genre) {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };
            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse resp = client.Get("@Genres/" + genre);
            Genre data = resp.ResultAs<Genre>();
            if (data == null) {
                FirebaseResponse response = client.Get("Genres");
                Dictionary<string, Genre> all_genres = response.ResultAs<Dictionary<string, Genre>>();
                if (all_genres == null) {
                    Genre new_genre = new Genre("1", genre);
                    response = client.Set(@"Genres/" + genre, new_genre);
                    return;
                }

                int count = all_genres.Count;
                int new_id = count + 1;
                String id = new_id.ToString();
                Genre new_genre_ = new Genre(id, genre);
                response = client.Set(@"Genres/" + genre, new_genre_);
            }
            // else no need to add genre as it already exists
        }

    }
}
