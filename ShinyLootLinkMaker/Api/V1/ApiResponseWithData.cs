using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Deserializers;

namespace ShinyLootLinkMaker.Api.V1
{
    public class ApiResponse<TCode, TResponse>
        : ApiResponse<TCode>
        where TResponse: class        
    {
        [DeserializeAs(Name = "data")]
        public TResponse Data { get; set; }
    }
}
