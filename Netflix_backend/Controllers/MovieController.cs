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

        [HttpDelete]
        public JsonResult Delete([FromQuery] String title) {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse resp = client.Get(@"Movies/" + title);
            MovieModel movie = resp.ResultAs<MovieModel>();
            if (movie != null) {
                resp = client.Delete(@"Movies/" + title);
                int status = (int)resp.StatusCode;
                if (status == 200) {
                    Response res = new Response(200, "Movie deleted successfully!");
                    return new JsonResult(res);
                }
                else
                {
                    Response res = new Response(400, "There was an error deleting the movie!");
                    return new JsonResult(res);
                }

            }
            else
            {
                Response res = new Response(400, "No such movie exists!");
                return new JsonResult(res);
            }

        }

        [HttpPost]
        public JsonResult Edit([FromBody] MovieModel movie) {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse resp = client.Get(@"Movies/" + movie.Title);
            MovieModel movie_ = resp.ResultAs<MovieModel>();
            if (movie_ != null) {
                resp = client.Update<MovieModel>(@"Movies/" + movie.Title, movie);
                int status = (int)resp.StatusCode;
                if (status == 200)
                {
                    Response res = new Response(200, "Movie updated successfully!");
                    return new JsonResult(res);
                }
                else
                {
                    Response res = new Response(400, "There was an error updating the movie!");
                    return new JsonResult(res);
                }
            }
            else
            {
                Response res = new Response(400, "No such movie exists!");
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
        }

        [HttpGet]
        public JsonResult GetByName([FromQuery] String name)
        {
            try {
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
            catch(Exception ex) {
                Response res = new Response(400, "Failed to fetch the movie!");
                return new JsonResult(res);
            }
        }

        [HttpGet]
        public JsonResult GetByGenre([FromQuery] String genre)  // single genre
        {
            try
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
                    for (int i = 0; i < genres.Length; i++)
                    {
                        if (genres[i] == genre)
                        {
                            movies.Add(value);
                        }

                    }

                }

                return new JsonResult(movies);
            }
            catch(Exception ex) {
                Response res = new Response(400, "Failed to fetch the movies!");
                return new JsonResult(res);
            }

            
        }

        [HttpPut]
        public JsonResult UpdateRating([FromQuery] String id, String rating) {
            try
            {
                IFirebaseConfig ifc = new FirebaseConfig()
                {
                    AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                    BasePath = "https://fir-fast-36fe8.firebaseio.com/"
                };


                IFirebaseClient client = new FirebaseClient(ifc);
                float rating_ = float.Parse(rating);
                FirebaseResponse resp = client.Get(@"Movies/" + id);
                MovieModel movie = resp.ResultAs<MovieModel>();
                movie.updateRating(rating_);
                resp = client.Update<MovieModel>(@"Movies/" + id, movie);
                Response res = new Response(200, "Rating successfully updated");
                return new JsonResult(res);
            }
            catch (Exception ex) {
                Response res = new Response(400, ex.Message);
                return new JsonResult(res);
            }

            
        }
    }
}
