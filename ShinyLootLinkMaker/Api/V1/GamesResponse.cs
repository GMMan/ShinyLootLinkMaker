using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Deserializers;

namespace ShinyLootLinkMaker.Api.V1
{
    public class GamesResponse
    {
        [DeserializeAs(Name = "games")]
        public List<Game> Games { get; set; }
    }
}
