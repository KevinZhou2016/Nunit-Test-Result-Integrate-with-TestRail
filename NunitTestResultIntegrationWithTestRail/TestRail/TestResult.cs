using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;
using NUnit.Core.Extensibility;


namespace TestRail
{
   public  class TestResult
    {              
       


        public string Result_message
       { get; set; }

        public string Result_stackTrace
        { get; set; }

        public string Result_status
        { get; set; }

        public string Case_id
        { get; set; }

        public string Suite_Id
        { get; set; }
        public string SuiteName
        { get; set; }
        public string Priority
        { get; set; }

        public string MileStone
        { get; set; }

        public string RunId
        { get; set; }
        public string Type
        { get; set; }


    }
}
