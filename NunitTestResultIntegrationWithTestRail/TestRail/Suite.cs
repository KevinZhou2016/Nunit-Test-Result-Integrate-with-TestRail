using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
namespace TestRail
{
   public class Suite
    {
        [JsonProperty(PropertyName = "project_id")]
       public string ProjectId { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string SuiteId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string SuiteName { get; set; }
    }
}
