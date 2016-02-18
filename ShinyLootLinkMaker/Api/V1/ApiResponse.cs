using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Deserializers;

namespace ShinyLootLinkMaker.Api.V1
{
    public class ApiResponse<TCode>
    {
        [DeserializeAs(Name = "code")]
        public TCode Code { get; set; }
        [DeserializeAs(Name = "message")]
        public string Message { get; set; }
    }
}
