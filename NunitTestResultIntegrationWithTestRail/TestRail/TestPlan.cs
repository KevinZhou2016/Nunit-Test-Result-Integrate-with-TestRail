using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TestRail
{
   
     public   class Run
        {
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }
            [JsonProperty(PropertyName = "name")]
            public string RunName { get; set; }
            [JsonProperty(PropertyName = "suite_id")]
            public string SuiteId { get; set; }
            [JsonProperty(PropertyName = "custom_status1_count")]
            public string AutomationPassedCount { get; set; }
            [JsonProperty(PropertyName = "custom_status2_count")]
            public string AutoamtionFailCount { get; set; }
            [JsonProperty(PropertyName = "untested_count")]
            public string UntestedCount { get; set; }
            public string url { get; set; }
        }

      public  class Entry
        {
            [JsonProperty(PropertyName = "runs")]
            public List<Run> Runs { get; set; }          
        }
    public    class Testplan
        {
            [JsonProperty(PropertyName = "entries")]
            public List<Entry> Entries { get; set; }
            public string Id {get;set;}
            [JsonProperty(PropertyName = "created_on")]
            public string CreateOn { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            [JsonProperty(PropertyName = "custom_status1_count")]
            public string AutomationPassedCount { get; set; }
            [JsonProperty(PropertyName = "custom_status2_count")]
            public string AutoamtionFailCount { get; set; }
            [JsonProperty(PropertyName = "untested_count")]
            public string UntestedCount { get; set; }
        }

      
    
}
