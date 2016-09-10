using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
namespace TestRail
{
   
    public class Test
    {
        public string Id { get; set; }
        public string PlanName { get; set; }
        [JsonProperty(PropertyName = "case_id")]
        public string  CaseId { get; set; }

        [JsonProperty(PropertyName = "run_id")]
        public string  RunId { get; set; }
        [JsonProperty(PropertyName = "status_id")]
        public string StatusId { get; set; }
        public string Title { get; set; }

    }

 

   
}
