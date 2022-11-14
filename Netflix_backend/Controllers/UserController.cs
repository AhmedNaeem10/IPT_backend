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

namespace Netflix_backend.Controllers
{
    public class User : Controller
    {
        public User()
        {

        }

        [HttpPost]
        public JsonResult Register([FromBody] UserRegister user)
        {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse resp = client.Get(@"Users/" + user.Email);
           
            UserModel user_ = resp.ResultAs<UserModel>();
            if (user_ == null) {
                FirebaseResponse users_res = client.Get("Users");
                Dictionary<string, UserModel> data = users_res.ResultAs<Dictionary<string, UserModel>>();
                int count = data.Count + 1;
                String id = count.ToString();
                UserModel new_user = new UserModel(id, user.Name, user.Email, user.Password);
                SetResponse set = client.Set(@"Users/" + new_user.Email, new_user);
                int status = (int)set.StatusCode;
                if (status == 200)
                {
                    Response res = new Response(status, "User successfully registered!");
                    return Json(res);
                }
                else
                {
                    Response res = new Response(status, "Failed to register the user!");
                    return Json(res);
                }
            }
            else
            {
                Response res = new Response(400, "User already exists!");
                return Json(res);
            }

            


        }

        [HttpPost]
        public JsonResult Login([FromBody] UserLogin user) {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };

           
            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse res = client.Get(@"Users/" + user.Email);
            UserModel user_ = res.ResultAs<UserModel>();
            if (user_.Password == user.Password) {
                return Json(user_);
            }
            return Json(null);

            
        }

        [HttpPut]
        public JsonResult AddToFavorites([FromQuery] String email, String movie)
        {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse res = client.Get(@"Users/" + email);
            UserModel user_ = res.ResultAs<UserModel>();
            if (user_ != null) {
                user_.Favlist.Add(movie);
                res = client.Update<UserModel>(@"Users/" + email, user_);
                Response response = new Response(200, "Movie successfully added to Favorites!");
                return Json(response);
            }
            else
            {
                Response response = new Response(400, "User does not exist!");
                return Json(response);
            }

        }

        [HttpPut]
        public JsonResult RemoveFromFavorites([FromQuery] String email, String movie)
        {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse res = client.Get(@"Users/" + email);
            UserModel user_ = res.ResultAs<UserModel>();
            if (user_ != null)
            {
                user_.Favlist.Remove(movie);
                res = client.Update<UserModel>(@"Users/" + email, user_);
                Response response = new Response(200, "Movie successfully removed from Favorites!");
                return Json(response);
            }
            else
            {
                Response response = new Response(400, "User does not exist!");
                return Json(response);
            }
        }

        [HttpPut]
        public JsonResult AddToHistory([FromQuery] String email, String movie)
        {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse res = client.Get(@"Users/" + email);
            UserModel user_ = res.ResultAs<UserModel>();
            if (user_ != null)
            {
                user_.MovieHistroy.Add(movie);
                res = client.Update<UserModel>(@"Users/" + email, user_);
                Response response = new Response(200, "Movie successfully removed from Favorites!");
                return Json(response);
            }
            else
            {
                Response response = new Response(400, "User does not exist!");
                return Json(response);
            }
        }

        [HttpPut]
        public JsonResult RemoveFromHistory([FromQuery] String email, String movie)
        {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse res = client.Get(@"Users/" + email);
            UserModel user_ = res.ResultAs<UserModel>();
            if (user_ != null)
            {
                user_.MovieHistroy.Remove(movie);
                res = client.Update<UserModel>(@"Users/" + email, user_);
                Response response = new Response(200, "Movie successfully removed from Favorites!");
                return Json(response);
            }
            else
            {
                Response response = new Response(400, "User does not exist!");
                return Json(response);
            }
        }

        [HttpPut]
        public JsonResult ClearHistory([FromQuery] String email, String movie)
        {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse res = client.Get(@"Users/" + email);
            UserModel user_ = res.ResultAs<UserModel>();
            if (user_ != null)
            {
                user_.MovieHistroy.Clear();
                res = client.Update<UserModel>(@"Users/" + email, user_);
                Response response = new Response(200, "Movie successfully removed from Favorites!");
                return Json(response);
            }
            else
            {
                Response response = new Response(400, "User does not exist!");
                return Json(response);
            }
        }

    }
}
