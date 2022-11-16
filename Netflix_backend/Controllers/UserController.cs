using System;
using FireSharp.Response;
using FireSharp.Config;
using Microsoft.AspNetCore.Mvc;
using FireSharp.Interfaces;
using FireSharp;
using Netflix_backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Firebase.Auth;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using System.Web;

namespace Netflix_backend.Controllers
{
    public class User : Controller
    {
        private static string ApiKey = "AIzaSyCPcdnvkK1SeroRhlgDkdA_EHPw4qHCluw";

        public User()
        {

        }

        [HttpPost]
        public async Task<JsonResult> Register([FromBody] UserRegister user)
        {
            try
            {
                var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));

                var a = await auth.CreateUserWithEmailAndPasswordAsync(user.Email, user.Password, user.Name, true);
                IFirebaseConfig ifc = new FireSharp.Config.FirebaseConfig()
                {
                    AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                    BasePath = "https://fir-fast-36fe8.firebaseio.com/"
                };

                IFirebaseClient client = new FirebaseClient(ifc);
                
                UserModel new_user = new UserModel(a.User.LocalId, user.Name, user.Email, user.Password);
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
            catch (Exception ex)
            {
                Response res = new Response(400, "User already exists or some other error!");
                return Json(res);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Login([FromBody] UserLogin user) {
            //IFirebaseConfig ifc = new FireSharp.Config.FirebaseConfig()
            //{
            //    AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
            //    BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            //};


            //IFirebaseClient client = new FirebaseClient(ifc);
            //FirebaseResponse res = client.Get("Users");
            //Dictionary<string, UserModel> data = res.ResultAs<Dictionary<string, UserModel>>();
            //foreach(KeyValuePair<string, UserModel> entry in data) {
            //    if (entry.Value.Email == user.Email && entry.Value.Password == user.Password) {
            //        UserModel usermodel = entry.Value;
            //        return JsonConvert.SerializeObject(usermodel);
            //    }

            //}
            //return "not";
            try
            {
                // Verification.
                if (ModelState.IsValid)
                {
                    

                    var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));
                    //FirebaseAuth auth_ = new FirebaseAuth();

                    

                    var ab = await auth.SignInWithEmailAndPasswordAsync(user.Email, user.Password);
                    await ab.RefreshUserDetails();
                    string token = ab.FirebaseToken;
                    var user_ = ab.User;
                    
                    
                    if (token != "")
                    {

                        //this.SignInUser(user.Email, token, false);
                        
                        Console.WriteLine(user_.IsEmailVerified);
                        return Json(user_);

                    }
                    else
                    {
                        // Setting.
                        return Json("Invalid username or password!");
                    }
                }
            }
            catch (Exception ex)
            {
                // Info
                return Json("Invalid username or password!");
            }
            return Json("There was an error in the request!");
        }

        [HttpPut]
        public JsonResult AddToFavorites([FromQuery] String uid, String movie)
        {
            IFirebaseConfig ifc = new FireSharp.Config.FirebaseConfig()
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
            IFirebaseConfig ifc = new FireSharp.Config.FirebaseConfig()
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
            IFirebaseConfig ifc = new FireSharp.Config.FirebaseConfig()
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
            IFirebaseConfig ifc = new FireSharp.Config.FirebaseConfig()
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
            IFirebaseConfig ifc = new FireSharp.Config.FirebaseConfig()
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


        [HttpGet]
        public String GetAll() {
            IFirebaseConfig ifc = new FireSharp.Config.FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse res = client.Get("Users");
            Dictionary<string, UserModel> data = res.ResultAs<Dictionary<string, UserModel>>();
            return JsonConvert.SerializeObject(data.Values);
        }

        [HttpGet]
        public String Get([FromQuery] String id)
        {
            IFirebaseConfig ifc = new FireSharp.Config.FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse res = client.Get(@"Users/" + id);
            UserModel user = res.ResultAs<UserModel>();
            return JsonConvert.SerializeObject(user);
        }

        [HttpGet]
        public async Task<JsonResult> ResetPassword([FromQuery] String email) {
            try
            { 
                if (ModelState.IsValid)
                {
                    var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));
                    await auth.SendPasswordResetEmailAsync(email);
                    return Json("Email has been sent!");
                }
            }
            catch (Exception ex)
            {
                return Json("There was an error in the request!");
            }
            return Json("There was an error in the request!");
        }

        [HttpPost]
        public async Task<JsonResult> SendVerificationEmail([FromBody] UserLogin user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));
                    var ab = await auth.SignInWithEmailAndPasswordAsync(user.Email, user.Password);
                    await ab.RefreshUserDetails();
                    string token = ab.FirebaseToken;
                    await auth.SendEmailVerificationAsync(token);
                    return Json("Email has been sent!");
                }
            }
            catch (Exception ex)
            {
                return Json("There was an error in the request!");
            }
            return Json("There was an error in the request!");
        }
    }
}
