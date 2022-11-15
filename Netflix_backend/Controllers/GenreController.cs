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
                Genre new_genre_ = new Genre(genre);
                FirebaseResponse response = client.Set(@"Genres/" + genre, new_genre_);
            }
            else {
                Console.WriteLine("Genre already exists");
            }

            // else no need to add genre as it already exists
        }

    }
}
