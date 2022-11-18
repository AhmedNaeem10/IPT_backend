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
using Newtonsoft.Json.Linq;
using FirebaseAdmin.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

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
            try
            {
                // Verification.
                if (ModelState.IsValid)
                {


                    var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));
                    var ab = await auth.SignInWithEmailAndPasswordAsync(user.Email, user.Password);
                    await ab.RefreshUserDetails();
                    
                    string token_ = ab.FirebaseToken;
                    
                    var user_ = ab.User;

                    if (token_ != "")
                    {
                        UserLoginResponse userLoginResponse = new UserLoginResponse(user_.LocalId, token_, user_.IsEmailVerified);
                        return Json(userLoginResponse);

                    }
                    else
                    {
                        return Json("Invalid username or password!");
                    }
                }
            }
            catch (Exception ex)
            {
                return Json("Invalid username or password!");
            }
            return Json("There was an error in the request!");
        }

        [HttpPut]
        public async Task<JsonResult> AddToFavorites([FromQuery] String uid, String movie, [FromHeader] String token)
        {
            try {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("/Users/ahmednaeem/Downloads/fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                    ProjectId = "fir-fast-36fe8",
                });

                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
              
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
            } catch (Exception ex) {
                Response response = new Response(400, "There was an error in the request!");
                return Json(response);
            }

            

        }

        [HttpPut]
        public async Task<JsonResult> RemoveFromFavorites([FromQuery] String uid, String movie, [FromHeader] String token)
        {
            try
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("/Users/ahmednaeem/Downloads/fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                    ProjectId = "fir-fast-36fe8",
                });

                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);

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
            catch (Exception ex) {
                Response response = new Response(400, "There was an error in the request!");
                return Json(response);
            }
        }

        [HttpPut]
        public async Task<JsonResult> AddToHistory([FromQuery] String uid, String movie, [FromHeader] String token)
        {
            try
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("/Users/ahmednaeem/Downloads/fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                    ProjectId = "fir-fast-36fe8",
                });

                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);

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
            catch (Exception ex) {
                Response response = new Response(400, "There was an error in the request!");
                return Json(response);
            }
        }

        [HttpPut]
        public async Task<JsonResult> RemoveFromHistory([FromQuery] String uid, String movie, [FromHeader] String token)
        {
            try {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("/Users/ahmednaeem/Downloads/fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                    ProjectId = "fir-fast-36fe8",
                });

                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);

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
            catch (Exception ex) {
                Response response = new Response(400, "There was an error in the request!");
                return Json(response);
            }

            
        }

        [HttpPut]
        public async Task<JsonResult> ClearHistory([FromQuery] String email, String movie, [FromHeader] String token)
        {
            try {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("/Users/ahmednaeem/Downloads/fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                    ProjectId = "fir-fast-36fe8",
                });

                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                string uid = decodedToken.Uid;

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
            catch (Exception ex) {
                Response response = new Response(400, "There was an error in the request!");
                return Json(response);
            }
        }


        [HttpGet]
        public async Task<String> GetAll([FromHeader] String token) {
            try {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("/Users/ahmednaeem/Downloads/fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                    ProjectId = "fir-fast-36fe8",
                });

                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                string uid = decodedToken.Uid;
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
            catch (Exception ex) {
                Response response = new Response(400, "There was an error in the request!");
                return JsonConvert.SerializeObject(response);
            }

            
        }

        [HttpGet]
        public async Task<String> Get([FromQuery] String id, [FromHeader] String token)
        {
            try {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("/Users/ahmednaeem/Downloads/fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                    ProjectId = "fir-fast-36fe8",
                });

                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                string uid = decodedToken.Uid;

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
            catch(Exception ex) {
                Response res = new Response(400, "There was an error in the request");
                return JsonConvert.SerializeObject(res);
            }
        }

        [HttpGet]
        public async Task<JsonResult> ResetPassword([FromQuery] String email, [FromHeader] String token) {
            try
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("/Users/ahmednaeem/Downloads/fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                    ProjectId = "fir-fast-36fe8",
                });

                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                string uid = decodedToken.Uid;
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
        public async Task<JsonResult> SendVerificationEmail([FromBody] UserLogin user, [FromHeader] String token)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));
                    var ab = await auth.SignInWithEmailAndPasswordAsync(user.Email, user.Password);
                    await ab.RefreshUserDetails();
                    string token_ = ab.FirebaseToken;
                    await auth.SendEmailVerificationAsync(token_);
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
