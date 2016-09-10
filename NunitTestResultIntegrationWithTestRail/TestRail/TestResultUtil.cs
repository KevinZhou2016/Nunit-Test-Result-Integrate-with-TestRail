using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Gurock.TestRail;
using Newtonsoft.Json;
using System.Configuration;
using Cryptography;

namespace TestRail
{
   public  class TestResultUtil
    {


        APIClient client = null;
        public TestResultUtil()
        {
            string TestRailServer = "https://kzhou3.testrail.net";        
            client = new APIClient(TestRailServer);                     
            string username = "usename";
            string password = "password";
            string securityKey = "XXX";
            client.User =Cryptography.Cryptography.Decrypt(username, securityKey);
            client.Password = Cryptography.Cryptography.Decrypt(password, securityKey);


        }
        public TestResultUtil(string username,string password)
        {
            string TestRailServer = "https://kzhou3.testrail.net";           
            client = new APIClient(TestRailServer);            
            string securityKey = "XXX";
            client.User = Cryptography.Cryptography.Decrypt(username, securityKey);
            client.Password = Cryptography.Cryptography.Decrypt(password, securityKey);


        }

        public TestResultUtil(string username, string password, string securityKey)
        {
            string TestRailServer = "https://kzhou3.testrail.net"; 
            client = new APIClient(TestRailServer);
            client.User = username;
            client.Password = Cryptography.Cryptography.Decrypt(password, securityKey);

        }

        



        public List<Testplan> GetDailyAutomationTestPlanID(string projectName, string planName)
        {
            string projectid = GetProjectIDByProjectName(projectName);
            JContainer TestPlans = GetTestPlansByProjectId(projectid);
            List<Testplan> ls_plan = new List<Testplan>();
            string planId = string.Empty;
            List<string> ls_planId = new List<string>();
            for (int i = 0; i < TestPlans.Count; i++)
            {

                if (TestPlans[i]["name"].ToString().Contains(planName) && TestPlans[i]["is_completed"].ToString() == "False")
                {                   
                    Testplan tp = JsonConvert.DeserializeObject<Testplan>(TestPlans[i].ToString());
                    ls_plan.Add(tp);
                }

            }

            return ls_plan;

        }


        public void CloseCompletedTestPlan(string planid)
        {
            string apiRequestStr = "close_plan/" + planid;

            JContainer cases = (JContainer)client.SendPost(apiRequestStr, string.Empty);
        }

        public  string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
         public bool Authvalidation()
         {             
             string apiRequestStr = "get_user_by_email&email=kzhou@greendotcorp.com";             
             JContainer user = (JContainer)client.SendGet(apiRequestStr);
             if (user == null)
             {

                 Console.WriteLine("Email/Login or Password is incorrect. Please try again.");
                 return false;
             }
             else
             {
                 return true;
             }
         }

         public void getuser()
         {
             //string apiRequestStr = "get_user/1";
             string apiRequestStr = "get_user_by_email&email=qa_test_automation@greendotcorp.com";
             JContainer user = (JContainer)client.SendGet(apiRequestStr);
         }

       public bool AddTestResult(TestResult trs)
       {

           bool res=false;
           //var data2 = new List<object>();
           //data2.Add(new { case_id = 1, status_id = 1 });
           //data2.Add(new { case_id = 2, status_id = 2 });

           //var data = new Dictionary<string, object>
           // {
              
           //      {"results" ,data2},
                 
                            
           // };
           string  statuskey=string.Empty;
           if (trs.Result_status =="Success")
               statuskey = "6";
           else
               statuskey = "7";

           trs.Result_message=trs.Result_message.Replace("\0",string.Empty);
           var data = new Dictionary<string, object>
           {
              {"case_id" ,trs.Case_id},
               {"status_id" ,statuskey},
               {"comment",trs.Result_message}
           };


           string caseId = string.Empty;
          // string apiRequestStr = "add_results_for_cases/1";
           //string apiRequestStr = "add_results/1";
           string apiRequestStr = "add_result_for_case/"+trs.RunId+"/"+trs.Case_id;
           
           JContainer result = (JContainer)client.SendPost(apiRequestStr, data);

           if (result != null)
           {
                res = true;


           }
           return res;
       }
       /*
       public bool AddTestResults(List<TestResult> trs_ls)
       {

           bool res = false;
           List<object> data2 = new List<object>();
           string runid=string.Empty;
           string caseId = string.Empty;
           string apiRequestStr = string.Empty;
           Dictionary<string, object> data = null;
           JContainer result = null;
           foreach (TestResult tr in trs_ls)
           {              
               string statuskey = string.Empty;
               if (tr.Result_status == "Passed")
                   statuskey = "6";
               else
                   statuskey = "7";
               
                if (runid == tr.Testrun_id||runid==string.Empty)
                {
                     runid = tr.Testrun_id;
                    data2.Add(new { case_id = tr.Case_id, comment = tr.Result_message + "\r\n" + tr.Result_stackTrace, status_id = statuskey });
                }
                else
                {

                    data = new Dictionary<string, object> { { "results", data2 } };
                    caseId = string.Empty;
                    apiRequestStr = "add_results_for_cases/" + runid;
                     result = (JContainer)client.SendPost(apiRequestStr, data);

                    if (result != null)
                    {
                        res = true;


                    }
                    data2.Clear();
                    data2.Add(new { case_id = tr.Case_id, comment = tr.Result_message + "\r\n" + tr.Result_stackTrace, status_id = statuskey });
                    runid = tr.Testrun_id;
                
                }
               
           }
           
            data = new Dictionary<string, object>{                        {"results" ,data2}                   };
                  caseId = string.Empty;
                  apiRequestStr = "add_results_for_cases/" +runid;
                  result = (JContainer)client.SendPost(apiRequestStr, data);

                 if (result != null)
                 {
                     res = true;


                 }
           


           //var data = new Dictionary<string, object>
           //{
           //   {"case_id" ,trs.Case_id},
           //    {"status_id" ,statuskey},
           //    {"comment",trs.Result_message+"\r\n"+trs.Result_stackTrace}
           //};

           
           
           //string apiRequestStr = "add_results/1";
           //string apiRequestStr = "add_result_for_case/" + trs.Testrun_id + "/" + trs.Case_id;

         
           return res;
       }
       */
       public string GetProjectIDByProjectName(string projectName)
       {
           string apiRequestStr = "get_projects";
           string projectId = string.Empty;
           JContainer result = (JContainer)client.SendGet(apiRequestStr);
           for (int i = 0; i < result.Count; i++)
           {
               if (result[i]["name"].ToString() == projectName)
               {
                   projectId = result[i]["id"].ToString();
                   return projectId;
               }
           
           }
           return projectId;

           
       
       }

       public Project GetProjectProjectId(string projectId)
       {
           string apiRequestStr = "get_project/" + projectId;
           JContainer result = (JContainer)client.SendGet(apiRequestStr);
           Project project = JsonConvert.DeserializeObject<Project>(result.ToString());
           return project;



       }

       public bool UpdateTestCase(string caseid, TestRail.LocalTestCase ltc)
       {
           bool result = false;
           var data = new Dictionary<string, object>
           {
              {"title" ,ltc.Title},              
               {"custom_cucumber",ltc.Steps}
           };

           string apiRequestStr = "update_case/" + caseid;

           JContainer cases = (JContainer)client.SendPost(apiRequestStr, data);
           if (cases != null)
           {
               result = true;
           }
           return result;
       }

       public bool UpdateAutomationTypeToSpecFlow(string caseid)
       {
           bool result = false;
           var data = new Dictionary<string, object>
           {            
               {"custom_automation_type",4}
           };

           string apiRequestStr = "update_case/" + caseid;

           JContainer cases = (JContainer)client.SendPost(apiRequestStr, data);
           if (cases != null)
           {
               result = true;
           }
           return result;
       }


       public bool UpdateAutomationType(string caseid,string type)
       {
           bool result = false;
           var data = new Dictionary<string, object>
           {            
               {"custom_automation_type",type}
           };

           string apiRequestStr = "update_case/" + caseid;

           JContainer cases = (JContainer)client.SendPost(apiRequestStr, data);
           if (cases != null)
           {
               result = true;
           }
           return result;
       }


       public bool UpdateAutomationType(string caseid, Dictionary<string, object> data)
       {
           bool result = false;          
           string apiRequestStr = "update_case/" + caseid;

           JContainer cases = (JContainer)client.SendPost(apiRequestStr, data);
           if (cases != null)
           {
               result = true;
           }
           return result;
       }

       public Case GetCaseByCaseId(string caseid)
       {
          
               string apiRequestStr = "get_case/" + caseid;


               JContainer result = (JContainer)client.SendGet(apiRequestStr);
               Case cs = JsonConvert.DeserializeObject<Case>(result.ToString());
               return cs;
          

       }

       public Suite GetSuiteBySuiteId(string suiteId)
       {
           string apiRequestStr = "get_suite/" + suiteId;


           JContainer result = (JContainer)client.SendGet(apiRequestStr);
           Suite suite = JsonConvert.DeserializeObject<Suite>(result.ToString());
           return suite;

       }
       public JContainer GetTestPlansByProjectId(string projectId)
       {

           string apiRequestStr = "get_plans/"+projectId;

           JContainer result = (JContainer)client.SendGet(apiRequestStr);

           return result;
       }

       public List<Case> GetTestCasesBySuiteID(string projectID, string suiteID)
       {
           List<Case> cases = new List<Case>();
           string apiRequestStr = String.Format("get_cases/{0}&suite_id={1}", projectID, suiteID);
           JContainer result = (JContainer)client.SendGet(apiRequestStr);
           for (int i = 0; i < result.Count; i++)
           {
               Case cs = new Case();
               cs.id = result[i]["id"].ToString();
               cs.Steps = result[i]["custom_steps"].ToString();
               cs.SuiteId = result[i]["suite_id"].ToString();
               cs.Title = result[i]["title"].ToString();
               cs.Type = result[i]["type_id"].ToString();
               cs.AutomationType = result[i]["custom_automation_type"].ToString();
               cs.TestCaseCategory=new List<string>();

               if (result[i]["custom_cust_test_case_cat"] != null)
               {
                   string[] cs_category = result[i]["custom_cust_test_case_cat"].ToString().Trim('[',']').Split(',');
                   foreach (string str in cs_category)
                   { 
                     cs.TestCaseCategory.Add(str.Replace("\r\n",string.Empty).Trim());
                   }
                
               }
               if (cs.AutomationType == "4")
               {
                   cases.Add(cs);
               }               
              
           }


           return cases;
       }

       public List<Case> GetTestCasesBySuiteIDAndSecionId(string projectID, string suiteID, string sectionid)
       {
           string apiRequestStr = String.Format("get_cases/{0}&suite_id={1}&section_id={2}", projectID, suiteID,sectionid);
           JContainer result = (JContainer)client.SendGet(apiRequestStr);
           List<Case> ls_cs = JsonConvert.DeserializeObject<List<Case>>(result.ToString());
           List<Case> ls_cs2 = new List<Case>();
           foreach(Case cs in ls_cs)
           {
               if (cs.AutomationType == "4")
               {
                   ls_cs2.Add(cs);
               }   
           }
           return ls_cs2;
       }

       public JContainer GetTestCaseByCaseID(string caseID)
       {
           string apiRequestStr = String.Format("get_case/{0}", caseID);
           return (JContainer)client.SendGet(apiRequestStr);
       }

       public Case GetTestCaseByCaseID2(string caseID)
       {
           string apiRequestStr = String.Format("get_case/{0}", caseID);
           JContainer result=(JContainer)client.SendGet(apiRequestStr);
           Case cs = JsonConvert.DeserializeObject<Case>(result.ToString());
           return cs;
       }


       public List<Section> GetSectionBysuiteId(string projectid,string suiteid)
       {
           string apiRequestStr = String.Format("get_sections/{0}&suite_id={1}", projectid, suiteid);
           JContainer result = (JContainer)client.SendGet(apiRequestStr);
           List<Section> se_ls = JsonConvert.DeserializeObject<List<Section>>(result.ToString());
           return se_ls;
       }

       public string GetTestPlanIDByProjectNameAndPlanName(string projectName,string planName)
       {
           string projectid=GetProjectIDByProjectName(projectName);
           JContainer TestPlans = GetTestPlansByProjectId(projectid);
           string planId = string.Empty;
           for (int i = 0; i < TestPlans.Count; i++)
           {
              
               if (TestPlans[i]["name"].ToString() == planName&&TestPlans[i]["is_completed"].ToString()=="False")
               {
                   planId = TestPlans[i]["id"].ToString();
                   return planId;
               }

           }
           
           return planId;
       }


       public Testplan GetTestPlanByProjectNameAndPlanName(string projectName, string planName)
       {

           string planid = GetTestPlanIDByProjectNameAndPlanName(projectName, planName);


           string apiRequestStr = "get_plan/" + planid;
           JContainer result = (JContainer)client.SendGet(apiRequestStr);
           Testplan tp = JsonConvert.DeserializeObject<Testplan>(result.ToString());               
           return tp;
       }

       public string GetRunIdByProjectNameAndPlanNameAndSuiteId(string projectName, string planName,string suiteid)
       {
           Testplan tp = GetTestPlanByProjectNameAndPlanName(projectName, planName);
           List<Entry> enties = tp.Entries;
           string runid = string.Empty;
           foreach (Entry entry in enties)
           {
               foreach (var r in entry.Runs)
               {
                   if (r.SuiteId==suiteid)
                   {
                       runid = r.Id;
                   }
               }
           }
           
           return runid;
       
       }

       public string GetSuiteIdByCaseId(string caseid)
       {
           string suiteid = string.Empty;
            string apiRequestStr = "get_case/" + caseid;
            try
            {

                JContainer result = (JContainer)client.SendGet(apiRequestStr);
                Case cs = JsonConvert.DeserializeObject<Case>(result.ToString());
                suiteid = cs.SuiteId;
                return suiteid;
            }
            catch (Exception ex)
            {
                throw ex;
            }

         
       
       }

       public List<Run> GetTestRunsByPlanID(string planId)
       {
           if (planId == string.Empty) return null;
           string apiRequestStr = "get_plan/" +planId;
           JContainer  result =(JContainer) client.SendGet(apiRequestStr);

         //dynamic dynObj=JsonConvert.DeserializeObject(result.ToString());
           //for(int i=0;i<result["entries"]
           List<string> RunIds_ls = new List<string>();
           List<Run> run_ls = new List<Run>();
           string runId = string.Empty;
          // var cls1 = JsonConvert.DeserializeObject<Class1>(result.ToString());
           var cls1 = JsonConvert.DeserializeObject<Testplan>(result.ToString());
           var ids = new List<Testplan>();
           foreach (var e in cls1.Entries)
           {
               foreach (var r in e.Runs)
               {
                   //run= r.Id;
                   run_ls.Add(r);
               }
           }

           return run_ls;
       }

       public List<Test> GetTestRunsByPlanName(string ProjectName, string planName)
       { 
       string projectId=GetProjectIDByProjectName(ProjectName);
       string planId = GetTestPlanIDByProjectNameAndPlanName(ProjectName, planName);
       List<Run> runs = GetTestRunsByPlanID(planId);
       if (runs == null) return null;
       List<Test> test_ls = new List<Test>(); 
       foreach (Run run in runs)
       {
           //test_ls = tru.GetTestByRunId(runId);
           test_ls.AddRange(GetTestsByRunId(run.Id));
       }
       foreach (Test t in test_ls)
       {
           t.PlanName = planName;
       }
       return test_ls;

       }


       public List<Run> GetRunIdsByProjectNameAndPlanNameAndSuiteId(string projectName, string planName, string suiteid)
       {
           Testplan tp = GetTestPlanByProjectNameAndPlanName(projectName, planName);
           List<Entry> enties = tp.Entries;
           string runid = string.Empty;
           List<Run> ls_runs = new List<Run>();
           foreach (Entry entry in enties)
           {
               foreach (var r in entry.Runs)
               {
                   if (r.SuiteId == suiteid)
                   {
                       ls_runs.Add(r);
                   }
               }
           }

           return ls_runs;

       }


       public List<Test> GetTestsByRunId(string runId)
       {
           if (runId == string.Empty) return null;
           string apiRequestStr = "get_tests/" +runId;
           JContainer result = (JContainer)client.SendGet(apiRequestStr);
           List<Test> ts_ls = JsonConvert.DeserializeObject<List<Test>>(result.ToString());           
           return ts_ls;
       }

       public void CreateTestPlan(string projectname,string planName,string milStone)
       {
           string projectId = GetProjectIDByProjectName(projectname);
           string apiRequestStr = "add_plan/"+projectId;

           
           Dictionary<string, object> data = new Dictionary<string, object>();
           data.Add("name", planName);           
           data.Add("entries", null);
           

           JContainer result = (JContainer)client.SendPost(apiRequestStr,data);
           
       }

       public void AddTestRun(string planName,string projectName,string suiteid ,string caseid)
       {
           string projectId = GetProjectIDByProjectName(projectName);             
               string planId = GetTestPlanIDByProjectNameAndPlanName(projectName, planName);
       Dictionary<string, object> data = new Dictionary<string,object>();
         List<object> data_runs = new List<object>();         
         List<object> runs = new List<object>();
         string[] caseids = new string[1];
         caseids[0] = caseid;
         //cases.Add(1);                  
           data.Add("suite_id",suiteid);
           //data.Add("assignedto_id",1);
           data.Add("include_all",false);
           data.Add("case_ids", caseids);           
             string apiRequestStr = "add_plan_entry/"+planId;
             
            JContainer result = (JContainer)client.SendPost(apiRequestStr,data);
           
           

       }


       public void AddTestRun2(string planName, string projectName, string suiteid, List<string> cases)
       {
           string projectId = GetProjectIDByProjectName(projectName);           
           string planId = GetTestPlanIDByProjectNameAndPlanName(projectName, planName);
           Dictionary<string, object> data = new Dictionary<string, object>();
           List<object> data_runs = new List<object>();
           List<object> runs = new List<object>();
           //cases.Add(1);                  
           data.Add("suite_id", suiteid);
           data.Add("assignedto_id", 6);
           data.Add("include_all", false);
           data.Add("case_ids", cases);
           string apiRequestStr = "add_plan_entry/" + planId;

           JContainer result = (JContainer)client.SendPost(apiRequestStr, data);



       }

       public List<Suite> GetSuitesByProjectName(string ProjectName)
       {
           List<Suite> suites = new List<Suite>();
           string projectId = GetProjectIDByProjectName(ProjectName);

           string apiRequestStr = "get_suites/"+projectId;           
           JContainer result = (JContainer)client.SendGet(apiRequestStr);
           for (int i = 0; i < result.Count; i++)
           {
               Suite suite = new Suite();
               suite.ProjectId = result[i]["project_id"].ToString();
               suite.SuiteId = result[i]["id"].ToString();
               suite.SuiteName = result[i]["name"].ToString();
               suites.Add(suite);
           }
           
           return suites;

       }

       public string GetSuiteIDBySuitename(string projectName,string suiteName)
       {
           List<Suite> Suites = GetSuitesByProjectName(projectName);
          string suiteid = string.Empty;
           for (int i = 0; i < Suites.Count; i++)
           {
              
               if (Suites[i].SuiteName.ToString().Contains(suiteName))
               {
                   suiteid = Suites[i].SuiteId.ToString();
                   return suiteid;
               }

           }
           
           return suiteid;
       }
       
       
    }

}
