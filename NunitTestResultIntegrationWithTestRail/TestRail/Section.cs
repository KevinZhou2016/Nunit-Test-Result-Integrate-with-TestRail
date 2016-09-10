using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TestRail
{
   public class Section
    {
        [JsonProperty(PropertyName = "id")]
        public string SectionId { get; set; }


        [JsonProperty(PropertyName = "parent_id")]
        public string ParentId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

    }
}
