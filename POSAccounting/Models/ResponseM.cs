using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
    public class ResponseM<T>
    {
        public HttpStatusCode Code { get; set; }
        public ErrorResponseM ErrorResponseM { get; set; }
        public T Model { get; set; }
    }

    public class ErrorResponseM
    {
        [JsonProperty(PropertyName = "Status")]
        public bool Status { get; set; }
        [JsonProperty(PropertyName = "Code")]
        public int Code { get; set; }
        [JsonProperty(PropertyName = "Message")]
        public string Message { get; set; }
    }
}
