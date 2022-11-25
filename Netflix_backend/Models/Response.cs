using System;
namespace Netflix_backend.Models
{
    public class Response
    {
        public int Code { get; set; }
        public dynamic Message { get; set; }
        public Response(int Code, dynamic Message)
        {
            this.Message = Message;
            this.Code = Code;
        }
    }
}
