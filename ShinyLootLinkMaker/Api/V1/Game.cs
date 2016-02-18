using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Deserializers;

namespace ShinyLootLinkMaker.Api.V1
{
    public class Game
    {
        [DeserializeAs(Name = "name")]
        public string Name { get; set; }
        [DeserializeAs(Name = "id")]
        public int ID { get; set; }
        [DeserializeAs(Name = "slug")]
        public string Slug { get; set; }
        [DeserializeAs(Name = "asset_dir")]
        public string AssetDir { get; set; }
        [DeserializeAs(Name = "cover_image_url")]
        public string CoverImageUrl { get; set; }
        [DeserializeAs(Name = "header_image_url")]
        public string HeaderImageUrl { get; set; }
        [DeserializeAs(Name = "protection_type_id")]
        public ProtectionType ProtectionType { get; set; }
        [DeserializeAs(Name = "dev_key_status")]
        public KeyStatus DevKeyStatus { get; set; }
        [DeserializeAs(Name = "steam_key_status")]
        public KeyStatus SteamKeyStatus { get; set; }
        [DeserializeAs(Name = "desura_key_status")]
        public KeyStatus DesuraKeyStatus { get; set; }
        [DeserializeAs(Name = "files")]
        public List<File> Files { get; set; }

        public Game()
        {
            Files = new List<File>();
        }
    }
}
