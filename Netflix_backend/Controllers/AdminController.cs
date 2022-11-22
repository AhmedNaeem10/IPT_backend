using System;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Firebase.Auth;
using FireSharp;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Netflix_backend.Models;

namespace Netflix_backend.Controllers
{
    public class AdminController: Controller
    {
        private static string ApiKey = "AIzaSyCPcdnvkK1SeroRhlgDkdA_EHPw4qHCluw";

        public AdminController()
        {
        }

        [HttpPost]
        public async Task<JsonResult> Login([FromBody] Admin admin) {
            try
            {
                // Verification.
                if (ModelState.IsValid)
                {
                    Console.WriteLine(admin.Email);


                    var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));
                    var ab = await auth.SignInWithEmailAndPasswordAsync(admin.Email, admin.Password);
                    await ab.RefreshUserDetails();

                    string token_ = ab.FirebaseToken;

                    var user_ = ab.User;

                    if (token_ != "")
                    {
                        Admin admin_ = new Admin(admin.Email, token: token_);
                        return new JsonResult(admin_);

                    }
                    else
                    {
                        Response res = new Response(401, "Invalid email or password");
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

        [HttpPost]
        public async Task<JsonResult> Register([FromBody] Admin admin) {
            try
            {
                var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));

                var a = await auth.CreateUserWithEmailAndPasswordAsync(admin.Email, admin.Password);
               
                Response res = new Response(200, "Admin successfully registered!");
                return Json(res);
            }
            catch (Exception ex)
            {
                Response res = new Response(400, "Invalid email or password!");
                return Json(res);
            }
        }

    }
}
