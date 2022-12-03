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
using FirebaseAuth = FirebaseAdmin.Auth.FirebaseAuth;
using Microsoft.Extensions.Configuration;
using FirebaseConfig = FireSharp.Config.FirebaseConfig;
using Microsoft.Extensions.Logging;

namespace Netflix_backend.Controllers
{
    
    public class User : Controller
    {
        private readonly ILogger<User> _logger;
        private static string ApiKey = "AIzaSyCPcdnvkK1SeroRhlgDkdA_EHPw4qHCluw";

        public User(ILogger<User> logger)
        {
            _logger = logger;
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
                new_user.UserId = a.User.LocalId;
                SetResponse set = client.Set(@"Users/" + new_user.UserId, new_user);
                int status = (int)set.StatusCode;
                if (status == 200)
                {
                    Response res = new Response(status, "User successfully registered!");
                    return Json(res);
                }
                else
                {
                    Response res = new Response(status, "Failed to register user!");
                    return Json(res);
                }

            }
            catch (Exception ex)
            {
                String msg = ex.Message;
                Console.WriteLine(msg);
                String part = msg.Split("Reason: ")[1];
                Response res = new Response(400, part);
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
                        Response res = new Response(401, "Could not issue firebase token");
                        return Json(res);
                    }
                }
            }
            catch (Exception ex)
            {
                Response res_ = new Response(401, "Invalid email or password");
                return Json(res_);
            }
            Response res__ = new Response(401, "Invalid email or password");
            return Json(res__);
        }

        [HttpPut]
        public async Task<JsonResult> AddToFavorites([FromQuery] String uid, String movie, [FromHeader] String Authorization)
        {
            try {
                String token = Authorization.Split(" ")[1];
                if (FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                        ProjectId = "fir-fast-36fe8",
                    });
                }

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
        public async Task<JsonResult> RemoveFromFavorites([FromQuery] String uid, String movie, [FromHeader] String Authorization)
        {
            try
            {
                String token = Authorization.Split(" ")[1];
                if (FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                        ProjectId = "fir-fast-36fe8",
                    });
                }

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
        public async Task<JsonResult> AddToHistory([FromQuery] String uid, String movie, [FromHeader] String Authorization)
        {
            try
            {
                String token = Authorization.Split(" ")[1];
                if (FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                        ProjectId = "fir-fast-36fe8",
                    });
                }

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
        public async Task<JsonResult> RemoveFromHistory([FromQuery] String uid, String movie, [FromHeader] String Authorization)
        {
            try {
                String token = Authorization.Split(" ")[1];
                if (FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                        ProjectId = "fir-fast-36fe8",
                    });
                }
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
        public async Task<JsonResult> ClearHistory([FromQuery] String email, String movie, [FromHeader] String Authorization)
        {
            try {
                String token = Authorization.Split(" ")[1];
                if (FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                        ProjectId = "fir-fast-36fe8",
                    });
                }

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
        public async Task<String> GetAll([FromHeader] String Authorization) { 
            try {
                String token = Authorization.Split(" ")[1];
                if (FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                        ProjectId = "fir-fast-36fe8",
                    });
                }
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
                if (data != null)
                {
                    return JsonConvert.SerializeObject(data.Values);
                }
                Response resp = new Response(200, "No User Exists");
                return JsonConvert.SerializeObject(resp);
             
                }
            catch (Exception ex) {
                Response response = new Response(400, "There was an error in the request!");
                return JsonConvert.SerializeObject(response);
            }

            
        }

        [HttpGet]
        public async Task<String> Get([FromQuery] String id, [FromHeader] String Authorization)
        {
            try {
                String token = Authorization.Split(" ")[1];
                if (FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                        ProjectId = "fir-fast-36fe8",
                    });
                }

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
        public async Task<JsonResult> ResetPassword([FromQuery] String email, [FromHeader] String Authorization) {
            try
            {
                String token = Authorization.Split(" ")[1];
                if (FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                        ProjectId = "fir-fast-36fe8",
                    });
                }

                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                string uid = decodedToken.Uid;
                if (ModelState.IsValid)
                {
                    var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));
                    await auth.SendPasswordResetEmailAsync(email);
                    Response res = new Response(200, "Password reset email has been sent!");
                    return Json(res);
                }
               
            }
            catch (Exception ex)
            {
                Response res = new Response(400, "Password reset email could not be sent!");
                return Json(res);
            }
            Response res_ = new Response(400, "Password reset email could not be sent!");
            return Json(res_);
        }
        [HttpGet]
        public async Task<JsonResult> checkVerification([FromQuery] String uid, [FromHeader] String Authorization)
        {
            try
            {
                if (FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                        ProjectId = "fir-fast-36fe8",
                    });
                }
                String token = Authorization.Split(" ")[1];
                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                string Uid = decodedToken.Uid;

                if (ModelState.IsValid)
                {
                    var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));
                    IFirebaseConfig ifc = new FireSharp.Config.FirebaseConfig()
                    {
                        AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                        BasePath = "https://fir-fast-36fe8.firebaseio.com/"
                    };
                    IFirebaseClient client = new FirebaseClient(ifc);
                    FirebaseResponse res = client.Get(@"Users/" + uid);
                    UserModel user = res.ResultAs<UserModel>();
                    var ab = await auth.SignInWithEmailAndPasswordAsync(user.Email, user.Password);
                    await ab.RefreshUserDetails();
                    string token_ = ab.FirebaseToken;
                    bool isVerified = ab.User.IsEmailVerified;
                    Response response = new Response(200, isVerified);
                    return Json(response);
                }
            }
            catch (Exception ex)
            {
                Response response_ = new Response(400, "There was an error in the request!");
                return Json(response_);
            }
            Response response__ = new Response(400, "There was an error in the request!");
            return Json(response__);
        }

        [HttpGet]
        public async Task<JsonResult> getSubscription([FromQuery] String uid, [FromHeader] String Authorization) {
            try
            {
                if (FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance == null) {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                        ProjectId = "fir-fast-36fe8",
                    });
                }
                String token = Authorization.Split(" ")[1];
                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                string Uid = decodedToken.Uid;
                if (ModelState.IsValid)
                {
                    var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));
                    IFirebaseConfig ifc = new FireSharp.Config.FirebaseConfig()
                    {
                        AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                        BasePath = "https://fir-fast-36fe8.firebaseio.com/"
                    };
                    IFirebaseClient client = new FirebaseClient(ifc);
                    FirebaseResponse res = client.Get(@"Users/" + uid);
                    UserModel user = res.ResultAs<UserModel>();
                    Response response = new Response(200, user.Subscription);
                    return Json(response);
                }
                }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Response response_ = new Response(400, "There was an error in the request!");
                return Json(response_);
            }
            Response response__ = new Response(400, "There was an error in the request!");
            return Json(response__);
        }

        [HttpGet]
        public async Task<JsonResult> AddSubscription([FromHeader] String Authorization,[FromQuery] String uid, String subscription)
        {
            try
            {
                if (FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                        ProjectId = "fir-fast-36fe8",
                    });
                }
                _logger.LogInformation(Authorization +" " +subscription + " " + uid);
                String token = Authorization.Split(" ")[1];
                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                string Uid = decodedToken.Uid;
                if (ModelState.IsValid)
                {
                    var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));
                    IFirebaseConfig ifc = new FireSharp.Config.FirebaseConfig()
                    {
                        AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                        BasePath = "https://fir-fast-36fe8.firebaseio.com/"
                    };
                    IFirebaseClient client = new FirebaseClient(ifc);
                    FirebaseResponse res = client.Get(@"Users/" + uid);
                    UserModel user = res.ResultAs<UserModel>();
                    user.Subscription = subscription;
                    res = client.Update<UserModel>(@"Users/" + uid, user);
                    Response response = new Response(200, "Subscription successfully added!");
                    return Json(response);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response response_ = new Response(400, "There was an error in the request!");
                return Json(response_);
            }
            Response response__ = new Response(400, "There was an error in the request!");
            return Json(response__);
        }


        [HttpGet]
        public async Task<JsonResult> SendVerificationEmail([FromQuery] String uid, [FromHeader] String Authorization)
        {
            try
            {
                if (FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                        ProjectId = "fir-fast-36fe8",
                    });
                }
                String token = Authorization.Split(" ")[1];
                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                string Uid = decodedToken.Uid;
                if (ModelState.IsValid)
                {
                    var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));
                    IFirebaseConfig ifc = new FireSharp.Config.FirebaseConfig()
                    {
                        AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                        BasePath = "https://fir-fast-36fe8.firebaseio.com/"
                    };
                    IFirebaseClient client = new FirebaseClient(ifc);
                    FirebaseResponse res = client.Get(@"Users/" + uid);
                    UserModel user = res.ResultAs<UserModel>();
                    var ab = await auth.SignInWithEmailAndPasswordAsync(user.Email, user.Password);
                    await ab.RefreshUserDetails();
                    string token_ = ab.FirebaseToken;
                    await auth.SendEmailVerificationAsync(token_);
                    Response response = new Response(200, "Verification email has been sent!");
                    return Json(response);
                }
            }
            catch (Exception ex)
            {
                Response res = new Response(400, "Verification email could not be sent!");
                return Json(res);
            }
            Response res_ = new Response(400, "Verification email could not be sent!");
            return Json(res_);
        }

        [HttpDelete]
        public async Task<JsonResult> Remove([FromQuery] String uid, [FromHeader] String Authorization)
        {
            IFirebaseClient client1;
            IFirebaseConfig ifc = new FireSharp.Config.FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };
            client1 = new FireSharp.FirebaseClient(ifc);
            try {
                
                if (FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                        ProjectId = "fir-fast-36fe8",
                    });
                }
                String token = Authorization.Split(" ")[1];
                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                string Uid = decodedToken.Uid;
                await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
                client1.Delete(@"Users/" + uid);
                Response res = new Response(200, "User successfully deleted!");
                return Json(res);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Response res = new Response(400, "User does not exist!");
                return Json(res);
            }
        }

        [HttpPut]
        public async Task<JsonResult> Update([FromBody] UserModel user, [FromHeader] String Authorization)
        {
            try
            {
                if (FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("fir-fast-36fe8-firebase-adminsdk-kktkq-a8801e9003.json"),
                        ProjectId = "fir-fast-36fe8",
                    });
                }
                String token = Authorization.Split(" ")[1];
                FirebaseToken decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                string Uid = decodedToken.Uid;

                var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));
                IFirebaseConfig ifc = new FireSharp.Config.FirebaseConfig()
                {
                    AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                    BasePath = "https://fir-fast-36fe8.firebaseio.com/"
                };
                IFirebaseClient client = new FirebaseClient(ifc);
                FirebaseResponse res = client.Get(@"Users/" + user.UserId);
                UserModel user_ = res.ResultAs<UserModel>();
                user_.Name = user.Name;
                user_.Avatar = user.Avatar;
                user_.Email = user.Email;
                user_.Restriction = user.Restriction;
                FirebaseResponse response = client.Update<UserModel>(@"Users/" + user.UserId, user_);
                Response resp = new Response(200, "User successfully updated!");
                return Json(resp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response res_ = new Response(400, "There was an error in the request!");
                return Json(res_);
            }
        }
    }
}
