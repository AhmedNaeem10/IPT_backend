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
using Newtonsoft.Json;

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
            UserLogin userlogin = new UserLogin(user.Email, user.Password);
            var response = Login(userlogin);
            if (response == null) {
                UserModel new_user = new UserModel(user.Name, user.Email, user.Password);
                SetResponse set = client.Set(@"Users/" + new_user.UserId, new_user);
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
        public String Login([FromBody] UserLogin user) {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };

           
            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse res = client.Get("Users");
            Dictionary<string, UserModel> data = res.ResultAs<Dictionary<string, UserModel>>();
            foreach(KeyValuePair<string, UserModel> entry in data) {
                if (entry.Value.Email == user.Email && entry.Value.Password == user.Password) {
                    UserModel usermodel = entry.Value;
                    return JsonConvert.SerializeObject(usermodel);
                }

            }
            return "not";

            
        }

        [HttpPut]
        public JsonResult AddToFavorites([FromQuery] String uid, String movie)
        {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse res = client.Get(@"Users/" + uid);
            UserModel user_ = res.ResultAs<UserModel>();
            if (user_ != null) {
                user_.Favlist.Add(movie);
                res = client.Update<UserModel>(@"Users/" + uid, user_);
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
        public JsonResult RemoveFromFavorites([FromQuery] String uid, String movie)
        {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse res = client.Get(@"Users/" + uid);
            UserModel user_ = res.ResultAs<UserModel>();
            if (user_ != null)
            {
                user_.Favlist.Remove(movie);
                res = client.Update<UserModel>(@"Users/" + uid, user_);
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
        public JsonResult AddToHistory([FromQuery] String uid, String movie)
        {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse res = client.Get(@"Users/" + uid);
            UserModel user_ = res.ResultAs<UserModel>();
            if (user_ != null)
            {
                user_.MovieHistroy.Add(movie);
                res = client.Update<UserModel>(@"Users/" + uid, user_);
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
        public JsonResult RemoveFromHistory([FromQuery] String uid, String movie)
        {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse res = client.Get(@"Users/" + uid);
            UserModel user_ = res.ResultAs<UserModel>();
            if (user_ != null)
            {
                user_.MovieHistroy.Remove(movie);
                res = client.Update<UserModel>(@"Users/" + uid, user_);
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
