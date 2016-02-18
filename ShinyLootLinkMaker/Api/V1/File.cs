using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Deserializers;

namespace ShinyLootLinkMaker.Api.V1
{
    public class File
    {
        [DeserializeAs(Name = "file_id")]
        public int ID { get; set; }
        [DeserializeAs(Name = "file_type_id")]
        public FileType Type { get; set; }
        [DeserializeAs(Name = "os_id")]
        public OSType OS { get; set; }
        [DeserializeAs(Name = "file_name")]
        public string FileName { get; set; }
        [DeserializeAs(Name = "short_description")]
        public string ShortDescription { get; set; }
        [DeserializeAs(Name = "file_size")]
        public long Size { get; set; }
        [DeserializeAs(Name = "created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
