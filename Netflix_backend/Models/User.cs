using System;
namespace Netflix_backend.Models
{
    public class UserModel
    {
        public String Username { get; set; }
        public String Password { get; set; }

        public UserModel() { }

        public UserModel(String username, String password)
        {
            this.Username = username;
            this.Password = password;
        }


    }
}
