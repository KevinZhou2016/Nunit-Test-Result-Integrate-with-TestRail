using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TestRail
{
   public class Project
    {
        [JsonProperty(PropertyName = "id")]
        public string ProjectId { get; set; }


        [JsonProperty(PropertyName = "name")]
        public string ProjectName { get; set; }
    }
}
