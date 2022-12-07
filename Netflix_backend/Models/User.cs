using System;
namespace Netflix_backend.Models
{
    public class UserLogin
    {
        public String Email { get; set; }
        public String Password { get; set; }

        public UserLogin() { }

        public UserLogin(String email, String password)
        {
            this.Email = email;
            this.Password = password;
        }


    }
}
