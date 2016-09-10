using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TestRail
{
   public class Case
    {
        [JsonProperty(PropertyName = "id")]
        public string id
        { get; set; }
        [JsonProperty(PropertyName = "suite_id")]
        public string SuiteId
        { get; set; }
        public string Title { get; set; }

        private string steps;
        [JsonProperty(PropertyName = "custom_cucumber")]
        public string Steps
        {
            get
            { 
                
                return steps; 
            }
           
            set
              {
                if(value==null)
                {
                  steps=string.Empty;
                }
                else
                {
                steps=value;
                }
             }
            
        }



        [JsonProperty(PropertyName = "type_id")]
        public string Type
        { get; set; }

        [JsonProperty(PropertyName = " custom_cust_test_case_cat")]
        public List<string> TestCaseCategory
        { get; set; }


        [JsonProperty(PropertyName = "custom_automation_type")]
        public string AutomationType
        {
            get;set;
        }

        [JsonProperty(PropertyName = "section_id")]
        public string SectionId
        {
            get;set;
        }
       
        private bool custom_cucumber_written;
        public bool Custom_cucumber_written
        {
            get
            {

                return custom_cucumber_written;
            }

            set
            {
                if (value == null)
                {
                    custom_cucumber_written = false;
                }
                else
                {
                    custom_cucumber_written = value;
                }
            }
        }
    }
  
}
