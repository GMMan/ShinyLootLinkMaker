using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Deserializers;

namespace ShinyLootLinkMaker.Api.V1
{
    public class LoginResponse
    {
        [DeserializeAs(Name = "token")]
        public string Token { get; set; }
    }
}
