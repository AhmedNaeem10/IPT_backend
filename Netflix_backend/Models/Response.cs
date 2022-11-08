using System;
namespace Netflix_backend.Models
{
    public class Response
    {
        public int Code { get; set; }
        public String Message { get; set; }
        public Response(int Code, String Message)
        {
            this.Message = Message;
            this.Code = Code;
        }
    }
}
