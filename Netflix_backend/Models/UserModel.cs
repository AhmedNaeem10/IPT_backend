using System;
using System.Collections.Generic;

namespace Netflix_backend.Models
{
    public class UserModel
    {
        public String UserId { get; set; }
    
        public String Name { get; set; }

        public String Email { get; set; }

        public String Password { get; set; }

        public String Subscription { get; set; }

        public String Restriction { get; set; }

        public String Avatar { get; set; }
        
        public List<String> Favlist;

        public List<String> MovieHistroy;

        public List<MovieGet> Recommendation;

        public UserModel()
        {
            this.Favlist = new List<String>();
            this.MovieHistroy = new List<String>();
            this.Recommendation = new List<MovieGet>();
        }

        public UserModel(String id, String name, String email, String password) {
            this.UserId = id;
            this.Name = name;
            this.Email = email;
            this.Password = password;
            this.Favlist = new List<String>();
            this.MovieHistroy = new List<String>();
            this.Recommendation = new List<MovieGet>();
            this.Subscription = "None";
            this.Avatar = "";
            this.Restriction = "PG-13";
        }

    }
}
