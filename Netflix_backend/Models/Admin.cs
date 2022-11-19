using System;
namespace Netflix_backend.Models
{
    public class Admin
    {
        public String Email { get; set; }
        public String Password { get; set; }
        public String Token { get; set; }
        public Admin()
        {
        }
        public Admin(String email, String password="", String token="") {
            Email = email;
            Password = password;
            Token = token;
        }
    }
}
