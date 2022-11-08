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
            FirebaseResponse resp = client.Get(@"Users/" + user.Username);
            UserRegister user_ = resp.ResultAs<UserRegister>();
            if (user_ == null) {
                SetResponse set = client.Set(@"Users/" + user.Username, user);
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
        public JsonResult Login([FromBody] UserModel user) {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };

           
            IFirebaseClient client = new FirebaseClient(ifc);
            FirebaseResponse res = client.Get(@"Users/" + user.Username);
            UserRegister user_ = res.ResultAs<UserRegister>();
            if (user_.Password == user.Password) {
                return Json(user_);
            }
            return null;

            
        }


        // just a testing function, nothing to do no
        public JsonResult New()
        {
            //Console.WriteLine(id);
            //string userid = UrlUtil.getParam(this, "userid", "");
            //string pwd = UrlUtil.getParam(this, "pwd", "");

            //string resp = DynAggrClientAPI.openSession(userid, pwd);
            //var jObject = JObject.Parse(resp);

            //var response = Request.CreateResponse(HttpStatusCode.OK);
            //response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
            //return response;
            return Json("Ahmed Naeem is a pure genius!");
        }
    }
}
