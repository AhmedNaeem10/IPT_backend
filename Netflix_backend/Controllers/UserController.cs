using System;
using System.Net.Http;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Config;
using Microsoft.AspNetCore.Mvc;
using FireSharp.Interfaces;
using FireSharp;
using Netflix_backend.Models;

namespace Netflix_backend.Controllers
{
    public class User : Controller
    {
        public User()
        {

        }

        
        public String Register()
        {

            // post request data, will handle later

            UserModel user = new UserModel("k191346@nu.edu.pk", "ahmed123");
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "VIB4QyeoIjd43kf2yFcU7l9ynqtKSJPF3fplsdUp",
                BasePath = "https://fir-fast-36fe8.firebaseio.com/"
            };


            IFirebaseClient client = new FirebaseClient(ifc);
            SetResponse set = client.Set(@"Users/Ahmed", user);
            return "user registered!";
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
