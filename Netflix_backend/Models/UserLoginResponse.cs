using System;
namespace Netflix_backend.Models
{
    public class UserLoginResponse
    {
        public String uid { get; set; }
        public bool isVerified { get; set; }
        public String token { get; set; }

        public UserLoginResponse(String uid, String token, bool isVerified)
        {
            this.uid = uid;
            this.token = token;
            this.isVerified = isVerified;
        }

    }
}
