﻿using System;
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
    public class MovieController
    {
        public MovieController()
        {
        }

        [HttpPost]
        public JsonResult Add([FromBody] MovieModel movie)
        {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse resp = client.Get(@"Movies/" + movie.Title);
            if (resp != null)
            {

                MovieModel movie_ = resp.ResultAs<MovieModel>();
                if (movie_ == null)
            {
                SetResponse set = client.Set(@"Movies/" + movie.Title, movie);
                int status = (int)set.StatusCode;
                if (status == 200)
                {
                    // now adding new genres if needed
                    String[] genres = movie.Genres;
                    for (int i = 0; i < genres.Length; i++) {
                        string genre = genres[i];
                        GenreController.Add(genre);
                    }

                    Response res = new Response(status, "Movie successfully added!");
                    return new JsonResult(res);
                }
                else
                {
                    Response res = new Response(status, "Failed to add the movie!");
                    return new JsonResult(res);
                }
            }
            else
            {
                Response response = new Response(400, "Movie already exists with given name!");
                return new JsonResult(response);
            }
            }
            else
            {
                Response res = new Response(400, "There was an error in the request!");
                return new JsonResult(res);
            }


        }

        [HttpGet]
        public JsonResult Get() {

            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse response = client.Get("Movies");
            Dictionary<string, MovieModel> data = response.ResultAs<Dictionary<string,  MovieModel>>();
            return new JsonResult(data);
            //MovieModel movies = response.ResultAs<MovieModel>();
            //return new JsonResult(movies);

        }

        [HttpGet]
        public JsonResult GetByName([FromQuery] String name)
        {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse resp = client.Get(@"Movies/" + name);
            MovieModel movie = resp.ResultAs<MovieModel>();
            return new JsonResult(movie);
        }

        [HttpGet]
        public JsonResult GetByGenre([FromQuery] String genre)  // single genre
        {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse resp = client.Get("Movies");
            Dictionary<string, MovieModel> data = resp.ResultAs<Dictionary<string, MovieModel>>();
            List<MovieModel> movies = new List<MovieModel>();
            foreach (KeyValuePair<string, MovieModel> entry in data)
            {
                MovieModel value = entry.Value;
                String[] genres = value.Genres;
                for (int i = 0; i < genres.Length; i++) {
                    if (genres[i] == genre) {
                        movies.Add(value);
                    }

                }

            }

            return new JsonResult(movies);
        }



    }
}
