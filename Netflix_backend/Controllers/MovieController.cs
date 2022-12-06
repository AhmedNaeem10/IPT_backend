using System;
using System.Net.Http;
using FireSharp.Config;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using FireSharp.Interfaces;
using FireSharp;
using Netflix_backend.Models;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Netflix_backend.Data;
using System.Linq;
using System.Security.Claims;
using static Google.Apis.Requests.BatchRequest;
using Firebase.Auth;
using FirebaseConfig = FireSharp.Config.FirebaseConfig;

namespace Netflix_backend.Controllers
{

    public class MovieController
    {
        private readonly IConfiguration configuration;
        private readonly Firebase.Database.FirebaseClient client;
        private readonly IFirebaseClient client1;
        private readonly string FireBaseDataBaseUrl;
        private readonly string ApiKey;
        private readonly string Bucket;
        private readonly string AuthEmail;
        private readonly string AuthPassword;
        public movie_rec recommendation_system;


        public MovieController(IConfiguration config)
        {
            configuration = config;

            IFirebaseConfig conn = new FirebaseConfig
            {
                AuthSecret = configuration.GetConnectionString("DataBaseSecret"),
                BasePath = configuration.GetConnectionString("DataBaseAuth")
            };
            FireBaseDataBaseUrl = configuration.GetConnectionString("DataBaseAuth");
            client1 = new FireSharp.FirebaseClient(conn);
            client = new Firebase.Database.FirebaseClient(FireBaseDataBaseUrl);
            ApiKey = configuration.GetConnectionString("WEBAPIKEY");
            Bucket = configuration.GetConnectionString("Bucket");
            AuthEmail = configuration.GetConnectionString("AuthEmail");
            AuthPassword = configuration.GetConnectionString("AuthPassword");
            recommendation_system = new movie_rec();
        }


        [HttpPost]
        public async Task<ActionResult> getFavMovies([FromBody] Class fav)
        {
            try
            {
                var movies = await client.Child("Movies").OnceAsync<MovieGet>();
                List<MovieGet> movieList = new List<MovieGet>();
                foreach (var a in movies)
                {
                    if (fav.favListIds.Contains(a.Object.MovieId))
                    {
                        movieList.Add(new MovieGet
                        {

                            MovieId = a.Object.MovieId,
                            Title = a.Object.Title,
                            Description = a.Object.Description,
                            TrailerUrl = a.Object.TrailerUrl,
                            PosterUrl = a.Object.PosterUrl,
                            ThumbnailUrl = a.Object.ThumbnailUrl,
                            Rating = a.Object.Rating,
                            Imdb = a.Object.Imdb,
                            MovieRating = a.Object.MovieRating,
                            Duration = a.Object.Duration,
                            Genres = a.Object.Genres,
                            Year = a.Object.Year,
                            createdOn = a.Object.createdOn
                        });
                    }
                }
                List<MovieGet> SortedList = movieList.OrderByDescending(o => o.createdOn).ToList();

                IDictionary<string, List<MovieGet>> dict_ = new Dictionary<string, List<MovieGet>>();
                dict_["Movies"] = SortedList;
                return new OkObjectResult(new { status = true, msg = "All Movie List", data = dict_ });

            }
            catch (Exception e)
            {
                return new OkObjectResult(new { status = false, msg = e.Message }) { StatusCode = 404 };
            }
        }
        [HttpPost]
        public async Task<IActionResult> addMovie([FromForm] MovieModel movie)
        {

            MovieGet movieGet = new MovieGet();
            try
            {
                FileUploader fileupload = new FileUploader();
                if (movie.TrailerFile.Length > 0)
                {

                    movieGet.TrailerUrl = await fileupload.FileUpload(movie.TrailerFile, ApiKey, AuthEmail, AuthPassword, Bucket);
                }
                else
                {
                    return new OkObjectResult(new { status = false, msg = "Movie Video is required" });
                }
                if (movie.PosterFile.Length > 0)
                {
                    movieGet.PosterUrl = await fileupload.FileUpload(movie.PosterFile, ApiKey, AuthEmail, AuthPassword, Bucket);
                }
                else
                {
                    return new OkObjectResult(new { status = false, msg = "Movie Poster is required" });
                }
                if (movie.ThumbnailFile.Length > 0)
                {
                    movieGet.ThumbnailUrl = await fileupload.FileUpload(movie.ThumbnailFile, ApiKey, AuthEmail, AuthPassword, Bucket);
                }
                else
                {
                    return new OkObjectResult(new { status = false, msg = "Movie Thumbail  is required" });
                }

                var allMovies = await client.Child("Movies").OnceAsync<MovieGet>();
                foreach (var a in allMovies)
                {
                    if (a.Object.Title == movie.Title)
                    {
                        return new OkObjectResult(new { status = false, msg = "Movie already exists with given name!" });
                    }
                }

                movieGet.MovieId = movie.MovieId;
                movieGet.Title = movie.Title;
                movieGet.Description = movie.Description;
                movieGet.Rating = movie.Rating;
                movieGet.Imdb = movie.Imdb;
                movieGet.Genres = movie.Genres;
                movieGet.MovieRating = movie.MovieRating;
                movieGet.Year = movie.Year;
                movieGet.Duration = movie.Duration;
                await client1.SetAsync(@"Movies/" + movie.MovieId, movieGet);
                return new OkObjectResult(new { status = true, msg = "Movie Added" });
            }
            catch (Exception e)
            {
                return new OkObjectResult(new { status = false, msg = e.Message }) { StatusCode = 404 };
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                FileUploader fileupload = new FileUploader();
                FirebaseResponse resp = client1.Get(@"Movies/" + id);

                MovieGet movie = resp.ResultAs<MovieGet>();
                if (movie != null)
                {
                    await fileupload.FileDelete(movie.TrailerUrl, ApiKey, AuthEmail, AuthPassword, Bucket);
                    await fileupload.FileDelete(movie.ThumbnailUrl, ApiKey, AuthEmail, AuthPassword, Bucket);
                    await fileupload.FileDelete(movie.PosterUrl, ApiKey, AuthEmail, AuthPassword, Bucket);
                    resp = client1.Delete(@"Movies/" + id);
                }
                else
                {
                    return new OkObjectResult(new { status = false, msg = "No Movie is Exist" });
                }
                return new OkObjectResult(new { status = true, msg = "Movie Deleted" });
            }
            catch (Exception)
            {
                return new OkObjectResult(new { status = true, msg = "There was an error in deleting a Movie" }) { StatusCode = 404 };
            }

        }

        [HttpPut]
        [ActionName("edit")]
        public async Task<IActionResult> Edit([FromForm] MovieModel movie, string id)
        {
            try
            {
                FirebaseResponse resp = client1.Get(@"Movies/" + id);
                MovieGet movieGet = new MovieGet();
                MovieGet movie_ = resp.ResultAs<MovieGet>();
                if (movie_ != null)
                {
                    FileUploader fileupload = new FileUploader();

                    if (movie.TrailerFile.Length > 0)
                    {

                        movieGet.TrailerUrl = await fileupload.FileUpload(movie.TrailerFile, ApiKey, AuthEmail, AuthPassword, Bucket);
                    }

                    if (movie.PosterFile.Length > 0)
                    {
                        movieGet.PosterUrl = await fileupload.FileUpload(movie.PosterFile, ApiKey, AuthEmail, AuthPassword, Bucket);
                    }

                    if (movie.ThumbnailFile.Length > 0)
                    {
                        movieGet.ThumbnailUrl = await fileupload.FileUpload(movie.ThumbnailFile, ApiKey, AuthEmail, AuthPassword, Bucket);
                    }

                    movieGet.MovieId = id;
                    movieGet.Title = movie.Title;
                    movieGet.Description = movie.Description;
                    movieGet.Rating = movie.Rating;
                    movieGet.Imdb = movie.Imdb;
                    movieGet.Genres = movie.Genres;
                    movieGet.MovieRating = movie.MovieRating;
                    movieGet.Year = movie.Year;
                    movieGet.Duration = movie.Duration;


                    resp = client1.Update<MovieGet>(@"Movies/" + id, movieGet);


                    return new OkObjectResult(new { status = true, msg = "Movie updated Successfully" });
                }
                else
                {

                    return new OkObjectResult(new { status = false, msg = "No such movie exists!" });
                }
            }
            catch (Exception)
            {
                return new OkObjectResult(new { status = false, msg = "There was an error updating the movie!" }) { StatusCode = 404 };
            }

        }



        [HttpGet]
        public async Task<ActionResult> getallMovies()
        {
            try
            {
                var movies = await client.Child("Movies").OnceAsync<MovieGet>();


                List<MovieGet> movieList = new List<MovieGet>();
                foreach (var a in movies)
                {
                    movieList.Add(new MovieGet
                    {

                        MovieId = a.Object.MovieId,
                        Title = a.Object.Title,
                        Description = a.Object.Description,
                        TrailerUrl = a.Object.TrailerUrl,
                        PosterUrl = a.Object.PosterUrl,
                        ThumbnailUrl = a.Object.ThumbnailUrl,
                        Rating = a.Object.Rating,
                        Imdb = a.Object.Imdb,
                        MovieRating = a.Object.MovieRating,
                        Duration = a.Object.Duration,
                        Genres = a.Object.Genres,
                        Year = a.Object.Year,
                        createdOn = a.Object.createdOn
                    });
                }
                List<MovieGet> SortedList = movieList.OrderByDescending(o => o.createdOn).ToList();

                IDictionary<string, List<MovieGet>> dict_ = new Dictionary<string, List<MovieGet>>();
                dict_["Movies"] = SortedList;
                return new OkObjectResult(new { status = true, msg = "All Movie List", data = dict_ });

            }
            catch (Exception e)
            {
                return new OkObjectResult(new { status = false, msg = e.Message }) { StatusCode = 404 };
            }
        }

        [HttpGet]
        public async Task<ActionResult> getRandomMovie()
        {
            try
            {
                var movies = await client.Child("Movies").OnceAsync<MovieGet>();


                List<MovieGet> movieList = new List<MovieGet>();
                foreach (var a in movies)
                {
                    movieList.Add(new MovieGet
                    {

                        MovieId = a.Object.MovieId,
                        Title = a.Object.Title,
                        Description = a.Object.Description,
                        TrailerUrl = a.Object.TrailerUrl,
                        PosterUrl = a.Object.PosterUrl,
                        ThumbnailUrl = a.Object.ThumbnailUrl,
                        Rating = a.Object.Rating,
                        Imdb = a.Object.Imdb,
                        MovieRating = a.Object.MovieRating,
                        Duration = a.Object.Duration,
                        Genres = a.Object.Genres,
                        Year = a.Object.Year,
                        createdOn = a.Object.createdOn
                    });
                }
                var random = new Random();
                int index = random.Next(movieList.Count);


                IDictionary<string, MovieGet> dict_ = new Dictionary<string, MovieGet>();
                dict_["Movies"] = movieList[index];
                return new OkObjectResult(new { status = true, msg = "Random Movie", data = dict_ });

            }
            catch (Exception e)
            {
                return new OkObjectResult(new { status = false, msg = e.Message }) { StatusCode = 404 };
            }
        }

        [HttpGet]
        public async Task<ActionResult> getMovies()
        {
            try
            {
                var movies = await client.Child("Movies").OnceAsync<MovieGet>();
                var genres = await client.Child("Genres").OnceAsync<Genre>();


                List<object> mL = new List<object>();
                foreach (var b in genres)
                {
                    IDictionary<string, List<MovieGet>> dict_ = new Dictionary<string, List<MovieGet>>();
                    List<MovieGet> movieList = new List<MovieGet>();

                    foreach (var a in movies)
                    {
                        if (a.Object.Genres.Contains(b.Object.Name))
                        {
                            movieList.Add(new MovieGet
                            {

                                MovieId = a.Object.MovieId,
                                Title = a.Object.Title,
                                Description = a.Object.Description,
                                TrailerUrl = a.Object.TrailerUrl,
                                PosterUrl = a.Object.PosterUrl,
                                ThumbnailUrl = a.Object.ThumbnailUrl,
                                Rating = a.Object.Rating,
                                Imdb = a.Object.Imdb,
                                MovieRating = a.Object.MovieRating,
                                Duration = a.Object.Duration,
                                Genres = a.Object.Genres,
                                Year = a.Object.Year,
                                createdOn = a.Object.createdOn
                            });
                        }
                    }
                    dict_[b.Object.Name] = movieList;
                    mL.Add(dict_);
                }


                // List<object> dict1 = new List<object>();
                //dict1.Add(dict_);
                return new OkObjectResult(new { status = true, msg = "All Movie List", data = mL });

            }
            catch (Exception e)
            {
                return new OkObjectResult(new { status = false, msg = e.Message }) { StatusCode = 404 };
            }
        }

        [HttpGet]
        public IActionResult getMovie(string id)
        {
            try
            {

                FirebaseResponse resp = client1.Get(@"Movies/" + id);
                MovieGet movie = resp.ResultAs<MovieGet>();

                IDictionary<string, object> dict_ = new Dictionary<string, object>();
                dict_["Movie"] = movie;
                return new OkObjectResult(new { status = true, msg = "Movie", data = dict_ });
            }
            catch (Exception e)
            {
                return new OkObjectResult(new { status = false, msg = e.Message }) { StatusCode = 404 };
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetByName([FromQuery] String name)
        {

            try
            {
                var movies = await client.Child("Movies").OnceAsync<MovieGet>();
                List<MovieGet> movieList = new List<MovieGet>();
                foreach (var a in movies)
                {
                    if (a.Object.Title.Equals(name))
                    {
                        movieList.Add(new MovieGet
                        {

                            MovieId = a.Object.MovieId,
                            Title = a.Object.Title,
                            Description = a.Object.Description,
                            TrailerUrl = a.Object.TrailerUrl,
                            PosterUrl = a.Object.PosterUrl,
                            ThumbnailUrl = a.Object.ThumbnailUrl,
                            Rating = a.Object.Rating,
                            Imdb = a.Object.Imdb,
                            MovieRating = a.Object.MovieRating,
                            Duration = a.Object.Duration,
                            Genres = a.Object.Genres,
                            Year = a.Object.Year,
                            createdOn = a.Object.createdOn
                        });
                    }
                }

                List<MovieGet> SortedList = movieList.OrderByDescending(o => o.createdOn).ToList();

                IDictionary<string, List<MovieGet>> dict_ = new Dictionary<string, List<MovieGet>>();
                dict_["Movies"] = SortedList;
                return new OkObjectResult(new { status = true, msg = "Movie List", data = dict_ });
            }
            catch (Exception)
            {
                return new OkObjectResult(new { status = false, msg = "No Movie Found" }) { StatusCode = 404 };
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByGenre([FromQuery] String genre)  // single genre
        {
            try
            {
                var movies = await client.Child("Movies").OnceAsync<MovieGet>();
                List<MovieGet> movieList = new List<MovieGet>();
                foreach (var a in movies)
                {
                    if (a.Object.Genres.Contains(genre))
                    {
                        movieList.Add(new MovieGet
                        {

                            MovieId = a.Object.MovieId,
                            Title = a.Object.Title,
                            Description = a.Object.Description,
                            TrailerUrl = a.Object.TrailerUrl,
                            PosterUrl = a.Object.PosterUrl,
                            ThumbnailUrl = a.Object.ThumbnailUrl,
                            Rating = a.Object.Rating,
                            Imdb = a.Object.Imdb,
                            MovieRating = a.Object.MovieRating,
                            Duration = a.Object.Duration,
                            Genres = a.Object.Genres,
                            Year = a.Object.Year,
                            createdOn = a.Object.createdOn
                        });
                    }
                }

                List<MovieGet> SortedList = movieList.OrderByDescending(o => o.createdOn).ToList();

                IDictionary<string, List<MovieGet>> dict_ = new Dictionary<string, List<MovieGet>>();
                dict_["Movies"] = SortedList;
                return new OkObjectResult(new { status = true, msg = "Movie List", data = dict_ });
            }
            catch (Exception)
            {

                return new OkObjectResult(new { status = false, msg = "No Movie Found" }) { StatusCode = 404 };
            }
        }

        [HttpGet]
        public JsonResult UpdateRating([FromQuery] String id, String rating)
        {
            try
            {

                float rating_ = float.Parse(rating);
                FirebaseResponse resp = client1.Get(@"Movies/" + id);
                MovieModel movie = resp.ResultAs<MovieModel>();
                movie.updateRating(rating_);
                resp = client1.Update<MovieModel>(@"Movies/" + id, movie);
                Response res = new Response(200, "Rating successfully updated");
                return new JsonResult(res);
            }
            catch (Exception ex)
            {
                Response res = new Response(400, ex.Message);
                return new JsonResult(res);
            }


        }
        [HttpGet]
        public async Task<JsonResult> Recommendation([FromQuery] String id, String uid)
        {
            try
            {
                var movies = await client.Child("Movies").OnceAsync<MovieGet>();
                List<MovieGet> movieList = new List<MovieGet>();
                foreach (var a in movies)
                {
                    movieList.Add(new MovieGet
                    {

                        MovieId = a.Object.MovieId,
                        Title = a.Object.Title,
                        Description = a.Object.Description,
                        TrailerUrl = a.Object.TrailerUrl,
                        PosterUrl = a.Object.PosterUrl,
                        ThumbnailUrl = a.Object.ThumbnailUrl,
                        Rating = a.Object.Rating,
                        Imdb = a.Object.Imdb,
                        MovieRating = a.Object.MovieRating,
                        Duration = a.Object.Duration,
                        Genres = a.Object.Genres,
                        Year = a.Object.Year,
                        createdOn = a.Object.createdOn
                    });
                }


                List<MovieGet> recommended_movies = recommendation_system.getRecommendation(id, movieList);
                var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));
                IFirebaseConfig ifc = new FireSharp.Config.FirebaseConfig()
                {
                    AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                    BasePath = "https://fir-fast-36fe8.firebaseio.com/"
                };
                IFirebaseClient client1 = new FirebaseClient(ifc);
                FirebaseResponse res = client1.Get(@"Users/" + uid);
                UserModel user = res.ResultAs<UserModel>();
                user.Recommendation = recommended_movies;
                client1.Update<UserModel>(@"Users/" + uid, user);
                Response response = new Response(200, "Recommendation successfully added!");
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                Response response = new Response(400, "There was an error in the request!");
                return new JsonResult(response);
            }

        }
    }
}
