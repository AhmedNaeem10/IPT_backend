using System;
namespace Netflix_backend.Models
{
    public class UserModel
    {
        public String username { get; set; }
        public String password { get; set; }
        public UserModel(String username, String password)
        {
            this.username = username;
            this.password = password;
        }


    }
}
