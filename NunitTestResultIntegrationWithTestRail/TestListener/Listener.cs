using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;
using NUnit.Core.Extensibility;
using TechTalk.SpecFlow;
using NUnit.Framework;
using System.Configuration;
using System.IO;
using System.Reflection;
using TestRail;


namespace TestListener
{

     [NUnitAddin(Name = "SampleAddIn", Description = "This is a Sample AddIn")]
    public class Listener : IAddin, EventListener
    {

                 
         TestRail.TestResultUtil tru =null;                  
         string project = string.Empty;
         string planName =string.Empty;         
         string output = string.Empty;
         string username=string.Empty;
         string password=string.Empty;
         string logPath = string.Empty;
         string runInJekins =string.Empty;         
         string  testCaseSync =string.Empty;
         string TestCaseSyncWithFailedTestCase = string.Empty;
         bool btestCaseSync = true;
         bool bTestCaseSyncWithFailedTestCase = true;
         string AddTestResult = string.Empty;
         bool bAddTestResult = true;
         string projectId = string.Empty;
         string runstarttime = string.Empty;
         string runname = string.Empty;
         List<TestRail.LocalTestCase> ltcs = new List<TestRail.LocalTestCase>();
         string failureTestCases = string.Empty;
            public bool Install(IExtensionHost host)
            {                
                IExtensionPoint listeners = host.GetExtensionPoint("EventListeners");
                if (listeners == null)
                    return false;              
                listeners.Install(this);
                return true;
            }





            public void RunStarted(string name, int testCount)
            {

                string suitename = Environment.GetEnvironmentVariable("SUITE_NAME");
                if (!Directory.Exists(Path.Combine(logPath, planName, "RunStatus")))
                {
                    Directory.CreateDirectory(Path.Combine(logPath, planName, "RunStatus"));
                }

                string[] files = Directory.GetFiles(Path.Combine(logPath, planName, "RunStatus"), "*_Runstatus.txt", SearchOption.TopDirectoryOnly);
                if (files.Length > 0 && suitename!=null)
                {
                    foreach (string file in files)
                    {
                        if (Path.GetFileNameWithoutExtension(file) == suitename + "_Runstatus")
                        {
                            File.Delete(file);
                        }
                    }
                }





                failureTestCases = string.Empty;
                username = string.Empty;
                password = string.Empty;
                logPath = string.Empty;
                 runInJekins = string.Empty;
                 testCaseSync = string.Empty;
                 btestCaseSync = true;
                 AddTestResult = string.Empty;
                 bAddTestResult = true;
                 bTestCaseSyncWithFailedTestCase = true;
                //get all test cases in local file
                // Console.WriteLine(DateTime.Now.ToLongTimeString());
                runstarttime = DateTime.Now.ToString("yyyyMMddHHmmss");
                string path = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", string.Empty);

                string[] dirs = Directory.GetFiles(path, "*_productIds.txt", SearchOption.AllDirectories);
                foreach(string dir2 in dirs)
                {
                    if (File.Exists(dir2))
                    {
                        File.Delete(dir2);
                    }
                }
               
                List<string> featureFiles = GetAllFile((Path.Combine(path, "RegreesionTests")));

                foreach (string str in featureFiles)
                {
                    ltcs.AddRange(GetLocalTestCase(str));
                    
                }               

                var localConfig = GetLocalConfig();
                project = Environment.GetEnvironmentVariable("TESTRAIL_PROJECT_NAME");
                if (project ==null)
                {
                    project = localConfig.Configs[0].Value;
                  
                                        
                }               
                planName = Environment.GetEnvironmentVariable("TEST_PLAN_NAME");
                if (planName == null)
                {
                    planName = localConfig.Configs[4].Value;
                    username = localConfig.Configs[5].Value;
                    password = localConfig.Configs[6].Value;
                }
                else
                {
                    username = "username encrpted";
                    password = "password encrpted";
                }
                Console.WriteLine("--_________________"+planName);

                runname = Environment.GetEnvironmentVariable("RUN_NAME");
                Console.WriteLine("--_________________"+runname);
                if (runname == null)
                {
                    runname = localConfig.Configs[8].Value;
                }
               
                logPath=Environment.GetEnvironmentVariable("LOGPATH");
                if (logPath == null)
                {
                   logPath = localConfig.Configs[1].Value;
                }
                Console.WriteLine("________path:" + logPath);
                Console.WriteLine("________project:" + project);                
                testCaseSync = localConfig.Configs[2].Value;
                AddTestResult = localConfig.Configs[3].Value;
                TestCaseSyncWithFailedTestCase = localConfig.Configs[7].Value;        
                
                if (AddTestResult.ToUpper() != "Y")
                {
                    bAddTestResult = false;
                }

                if (testCaseSync.ToUpper() != "Y")
                {
                    btestCaseSync = false;
                }

                if (TestCaseSyncWithFailedTestCase.ToUpper() != "Y")
                {
                    bTestCaseSyncWithFailedTestCase = false;
                }
                tru = new TestRail.TestResultUtil(username, password);
                projectId = tru.GetProjectIDByProjectName(project);
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                if (!Directory.Exists(Path.Combine(logPath, planName)))
                {
                    Directory.CreateDirectory(Path.Combine(logPath, planName));
                }

                if (!Directory.Exists(Path.Combine(logPath, "archive")))
                {
                    Directory.CreateDirectory(Path.Combine(logPath, "archive"));
                }

                DirectoryInfo dir = new DirectoryInfo(logPath);
                foreach (DirectoryInfo dirinfo in dir.GetDirectories())
                {
                    if (dirinfo.Name != "archive"&&dirinfo.CreationTime < DateTime.Now.AddDays(-60))
                    {
                            dirinfo.MoveTo(Path.Combine(logPath, "archive", dirinfo.CreationTime.ToString("yyyMMddHHmmss") + "." + dirinfo.Name));                        
                    }
                }   
        

            }




            public void RunFinished(Exception exception)
            {               

                //create run status file

                string suitename = Environment.GetEnvironmentVariable("SUITE_NAME");
                string path2 = Path.Combine(logPath, planName, "RunStatus", suitename + "_Runstatus");
                string content = string.Empty;

                if (suitename != null)
                {
                    content = string.Format("{0}:{1}", suitename, "exception Complete:exception");
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(path2))
                        {
                            sw.WriteLine(content);
                            sw.Close();

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception message:" + ex);

                    }
                }

            }

            public void RunFinished(NUnit.Core.TestResult result)
            {

                
             
                 //create run status file

                string suitename = Environment.GetEnvironmentVariable("SUITE_NAME");
                string path2=Path.Combine(logPath, planName,"RunStatus",suitename+ "_Runstatus.txt");
                string content=string.Empty;
                
                if (suitename != null)
                {
                    content = string.Format("{0}:{1}", suitename, "Complete");
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(path2))
                        {
                            sw.WriteLine(content);
                            sw.Close();

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception message:" + ex);

                    }
                }

            }

            public void SuiteFinished(NUnit.Core.TestResult result)
            {
                string testType= result.Test.TestType;

                if (testType =="ParameterizedTest")
                {
                    finalres = testcaseres[0];            
                    
                    foreach (bool res in testcaseres)
                    {
                        finalres &= res;
                    }
                    TestRail.TestResult tr = new TestRail.TestResult();                    
                    tr.Case_id = caseid;
                    if (finalres)
                    {
                        tr.Result_status = "Success";
                        tr.Result_message = "Final result -Success: \r\n";
                    }
                    else
                    {
                        tr.Result_status = "Failure";
                        tr.Result_message = "Final result -Failure: \r\n";
                    }
                    foreach (KeyValuePair<string,string> res in results)
                    {
                        tr.Result_message += res.Key + ":" + res.Value + "\r\n";
                    }

                    try
                    {

                       // List<string> runids = GetTestRunsByPlanNameAndSuiteId(project, planName, suiteid, tr.Case_id);
                        List<Run> runs = tru.GetRunIdsByProjectNameAndPlanNameAndSuiteId(project, planName,suiteid);                       
                        foreach (Run run in runs)
                        {
                            if (runname.Trim() == string.Empty)
                            {

                                tr.RunId = run.Id;
                                if (bAddTestResult)
                                {
                                    tru.AddTestResult(tr);
                                }
                                if (btestCaseSync)
                                {
                                    TestRail.Suite suite = null;
                                    TestRail.Case cs = null;
                                    TestRail.LocalTestCase ltc = null;
                                    bool updateresult = SyncTestcase(tr, ref ltc, ref cs, ref suite);
                                    if (updateresult)
                                    {

                                        string message = "Case update success, " + "Suite:" + suite.SuiteName + ", Case: " + tr.Case_id + ", Title:" + ltc.Title;
                                        AddLog(Path.Combine(logPath, planName, runstarttime + "." + planName + "_CaseUpdate.log"), message);
                                    }
                                }
                            }
                            else
                            {
                                if (run.RunName == runname.Trim())
                                {
                                    tr.RunId = run.Id;
                                    if (bAddTestResult)
                                    {
                                        tru.AddTestResult(tr);
                                    }
                                    if (btestCaseSync)
                                    {
                                        TestRail.Suite suite = null;
                                        TestRail.Case cs = null;
                                        TestRail.LocalTestCase ltc = null;
                                        bool updateresult = SyncTestcase(tr, ref ltc, ref cs, ref suite);
                                        if (updateresult)
                                        {

                                            string message = "Case update success, " + "Suite:" + suite.SuiteName + ", Case: " + tr.Case_id + ", Title:" + ltc.Title;
                                            AddLog(Path.Combine(logPath, planName, runstarttime + "." + planName + "_CaseUpdate.log"), message);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        caseid = tr.Case_id;
                        string errorMsg = result.FullName + ":\r\n" + "caseid:" + caseid + "\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n";
                        AddLog(Path.Combine(logPath,planName, runstarttime + "." + planName + "_ErrorMsg.log"), errorMsg);
                        throw ex;
                    }
            
                    
                }
                results.Clear();                
                testcaseres.Clear();
            }

            public void SuiteStarted(TestName testName)
            {
               
            }
            bool finalres = false;
            List<bool> testcaseres = new List<bool>();
            Dictionary<string, string> results = new Dictionary<string,string>();
            string caseid = string.Empty;
            string suiteid = string.Empty;
            public void TestFinished(NUnit.Core.TestResult result)
            {
                TestRail.TestResult tr = null;
                string testType = result.Test.TestType;

             
                try
                {
                                        
                    GetTestResult(result, ref tr, ref failureTestCases);
                    //update automation type to  specflow after test finished
                    TestRail.Case  cse= tru.GetCaseByCaseId(tr.Case_id);
                    if (cse.AutomationType != "4")
                    {
                        tru.UpdateAutomationTypeToSpecFlow(tr.Case_id);
                    }
                    
                    suiteid = tr.Suite_Id;
                    if (result.Test.Categories.Count == 0)
                    {
                        if (result.IsSuccess)
                        {
                            testcaseres.Add(true);

                        }
                        else
                        {
                            testcaseres.Add(false);
                        } 
                        results.Add(result.FullName, tr.Result_status.ToString());
                        caseid = tr.Case_id;
                       
                    }
                       
                      if (!result.IsSuccess)
                      {
                          
                            AddLog(Path.Combine(logPath,planName, runstarttime + "." + planName + "_FailureTestCases.log"), result.FullName);                          
                      }

                      List<Run> runs = tru.GetRunIdsByProjectNameAndPlanNameAndSuiteId(project, planName, tr.Suite_Id);

                      // List<string> runids = GetTestRunsByPlanNameAndSuiteId(project, planName, tr.Suite_Id, tr.Case_id);

                      #region
                      //if (runname.Trim() == string.Empty)
                      //{
                      //    foreach (Run run in runs)
                      //    {
                      //        TestRail.Suite suite = null;
                      //        TestRail.Case cs = null;
                      //        TestRail.LocalTestCase ltc = null;
                      //        output = string.Empty;
                      //        tr.RunId = run.Id;
                      //        if (bAddTestResult)
                      //        {
                      //            tru.AddTestResult(tr);
                      //        }
                      //        if (btestCaseSync && result.Test.Categories.Count == 1)
                      //        {
                      //            bool updateresult = SyncTestcase(tr, ref ltc, ref cs, ref suite);
                      //            if (updateresult)
                      //            {

                      //                string message = "Case update success, " + "Suite:" + suite.SuiteName + ", Case: " + tr.Case_id + ", Title:" + ltc.Title;
                      //                AddLog(Path.Combine(logPath, planName, runstarttime + "." + planName + "_CaseUpdate.log"), message);
                      //            }
                      //        }
                      //    }
                      //}
                      //else
                      //{
                      //    foreach (Run run in runs)
                      //    {
                      //        if (run.RunName == runname.Trim())
                      //        {
                      //            TestRail.Suite suite = null;
                      //            TestRail.Case cs = null;
                      //            TestRail.LocalTestCase ltc = null;
                      //            output = string.Empty;
                      //            tr.RunId = run.Id;
                      //            if (bAddTestResult)
                      //            {
                      //                tru.AddTestResult(tr);
                      //            }
                      //            if (btestCaseSync && result.Test.Categories.Count == 1)
                      //            {
                      //                bool updateresult = SyncTestcase(tr, ref ltc, ref cs, ref suite);
                      //                if (updateresult)
                      //                {

                      //                    string message = "Case update success, " + "Suite:" + suite.SuiteName + ", Case: " + tr.Case_id + ", Title:" + ltc.Title;
                      //                    AddLog(Path.Combine(logPath, planName, runstarttime + "." + planName + "_CaseUpdate.log"), message);
                      //                }
                      //            }
                      //        }
                      //    }
                      //}
                      #endregion

                      foreach (Run run in runs)
                      {
                          if (runname.Trim() == string.Empty)
                          {
                              TestRail.Suite suite = null;
                              TestRail.Case cs = null;
                              TestRail.LocalTestCase ltc = null;
                              output = string.Empty;
                              tr.RunId = run.Id;
                              if (bAddTestResult)
                              {
                                  tru.AddTestResult(tr);
                              }
                              if (btestCaseSync && result.Test.Categories.Count == 1)
                              {
                                  bool updateresult = SyncTestcase(tr, ref ltc, ref cs, ref suite);
                                  if (updateresult)
                                  {

                                      string message = "Case update success, " + "Suite:" + suite.SuiteName + ", Case: " + tr.Case_id + ", Title:" + ltc.Title;
                                      AddLog(Path.Combine(logPath, planName, runstarttime + "." + planName + "_CaseUpdate.log"), message);
                                  }
                              }
                          }
                          else 
                          {
                              if (run.RunName == runname.Trim())
                              {
                                  TestRail.Suite suite = null;
                                  TestRail.Case cs = null;
                                  TestRail.LocalTestCase ltc = null;
                                  output = string.Empty;
                                  tr.RunId = run.Id;
                                  if (bAddTestResult)
                                  {
                                      tru.AddTestResult(tr);
                                  }
                                  if (btestCaseSync && result.Test.Categories.Count == 1)
                                  {
                                      bool updateresult = SyncTestcase(tr, ref ltc, ref cs, ref suite);
                                      if (updateresult)
                                      {

                                          string message = "Case update success, " + "Suite:" + suite.SuiteName + ", Case: " + tr.Case_id + ", Title:" + ltc.Title;
                                          AddLog(Path.Combine(logPath, planName, runstarttime + "." + planName + "_CaseUpdate.log"), message);
                                      }
                                  }
                              }
                          }
                      }

                }
                catch (Exception ex)
                {
                    caseid = tr.Case_id;
                    string errorMsg = result.FullName + ":\r\n" + "caseid:" + caseid + "\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n";
                    AddLog(Path.Combine(logPath, planName, runstarttime + "." + planName + "_ErrorMsg.log"), errorMsg);
                    throw ex;
                }
            
            }

            

            public static List<string> GetAllFile(string path)
            {

                List<string> fileName = new List<string>();
                DirectoryInfo folder = new DirectoryInfo(path);
                TestRail.LocalTestCase ltc = new TestRail.LocalTestCase();

                foreach (FileInfo file in folder.GetFiles("*.feature", SearchOption.AllDirectories))
                {
                    fileName.Add(file.DirectoryName + "\\" + file.Name);
                }
                return fileName;
            }

            public static List<TestRail.LocalTestCase> GetLocalTestCase(string path)
            {             
                List<TestRail.LocalTestCase> list_localTestCase = new List<TestRail.LocalTestCase>();
                // TreeNode lastSection = null;
                StreamReader sr = new StreamReader(path, Encoding.Default);
                string line = string.Empty;
                string suiteName = string.Empty, casePath = string.Empty, lastSectionName = string.Empty, caseId = string.Empty, title = string.Empty, steps = string.Empty;

                StringBuilder sb = null;
                bool caseStartFlag = false;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.TrimStart().StartsWith("@caseid:") && !caseStartFlag)//caseid line
                    {
                        sb = new StringBuilder();
                        string[] tags=line.Split('@');
                        foreach (string tag in tags)
                        { 
                         if(tag.StartsWith("caseid:"))
                         {
                             caseId = tag.Replace("caseid:", string.Empty).Trim();
                         }

                        }                        
                        caseStartFlag = true;
                    }
                    else if (line.TrimStart().StartsWith("Scenario:") && caseStartFlag)
                    {

                        title = line.Replace("Scenario:", string.Empty);
                        //if (title == "Validate PinRule table for NewEveryday Visa")
                        //{
                        //}
                        //else
                        //{
                        //    sb.AppendLine(line.ToString());
                        //}
                    }
                    else if (line.TrimStart().StartsWith("Scenario Outline:") && caseStartFlag)
                    {

                        title = line.Replace("Scenario Outline:", string.Empty);
                        //if (title == "Validate PinRule table for NewEveryday Visa")
                        //{
                        //}
                        //else
                        //{
                        //    sb.AppendLine(line.ToString());
                        //}
                    }
                    else if (!line.TrimStart().StartsWith("#")&&!line.TrimStart().StartsWith("@caseid:") && !line.TrimStart().StartsWith("Scenario:") && caseStartFlag)
                    {
                        sb.AppendLine(line.ToString());
                    }
                    else if (line.TrimStart().StartsWith("@caseid:") && caseStartFlag)
                    {
                        //if (caseId == "1277464")
                        //{
                        //}
                        TestRail.LocalTestCase tc = new TestRail.LocalTestCase();
                        steps = sb.ToString();
                        tc.Title = title;
                        tc.CaseId = caseId;
                        tc.Steps = steps;
                        list_localTestCase.Add(tc);
                        sb.Clear();
                        //foreach (LocalTestCases tc2 in list_localTestCase)
                        //{
                        //    Console.WriteLine(tc2.CaseID);
                        //    Console.WriteLine(tc2.Title);
                        //    Console.WriteLine(tc2.Steps);
                        //}

                        string[] tags = line.Split('@');
                        foreach (string tag in tags)
                        {
                            if (tag.StartsWith("caseid:"))
                            {
                                caseId = tag.Replace("caseid:", string.Empty).Trim();
                            }

                        }            
                        // caseStartFlag =false;
                    }

                }

                if (line == null && caseId != string.Empty)
                {
                    TestRail.LocalTestCase tc = new TestRail.LocalTestCase();
                    tc.Title = title;
                    tc.CaseId = caseId;
                    steps = sb.ToString();
                    tc.Steps = steps;
                    //Console.WriteLine(tc.CaseID);
                    //Console.WriteLine(tc.Title);
                    //Console.WriteLine(tc.Steps);
                    list_localTestCase.Add(tc);
                }

                sr.Close();
                sr.Dispose();
                return list_localTestCase;
            }


            public bool SyncTestcase(TestRail.TestResult tr, ref TestRail.LocalTestCase ltc,ref TestRail.Case cs, ref TestRail.Suite suite)
            {
                bool updateResult = false;
                if (bTestCaseSyncWithFailedTestCase)
                {
                    ltc = ltcs.Find(p => p.CaseId == tr.Case_id);
                    cs = tru.GetCaseByCaseId(tr.Case_id);
                    suite = tru.GetSuiteBySuiteId(cs.SuiteId);
                    if (cs.AutomationType == "4" && suite.ProjectId == projectId)//retail project,automated type
                    {
                        if (ltc.Steps.Trim() != cs.Steps.Trim() || ltc.Title.Trim() != cs.Title.Trim())
                        {
                            //update test case in test rail
                            updateResult = tru.UpdateTestCase(ltc.CaseId, ltc);
                        }

                    }

                }
                else
                {
                    if (tr.Result_status == "Success")
                    {
                    ltc = ltcs.Find(p => p.CaseId == tr.Case_id);
                    cs = tru.GetCaseByCaseId(tr.Case_id);
                    suite = tru.GetSuiteBySuiteId(cs.SuiteId);
                    if (cs.AutomationType == "4" && suite.ProjectId == projectId)//retail project,automated type
                    {
                        if (ltc.Steps.Trim() != cs.Steps.Trim() || ltc.Title.Trim() != cs.Title.Trim())
                        {
                            //update test case in test rail
                            updateResult = tru.UpdateTestCase(ltc.CaseId, ltc);
                        }

                    }

                    }
                }
                return updateResult;
            }

            public void TestOutput(TestOutput testOutput)
            {
                output += testOutput.Text;
            }

            public void TestStarted(TestName testName)
            {
               // Console.WriteLine(testName.RunnerID);
                
            }

            public void UnhandledException(Exception exception)
            {
            }

            private void AddLog(string path,string errMessage)
            {
                //string path = @"c:\temp\MyTest.txt";
                // This text is added only once to the file. 

                if (!File.Exists(path))
                {

                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(errMessage);

                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(errMessage);
                    }
                }
                // Open the file to rea
            }
            private LocalConfig GetLocalConfig()
            {
                var configFileFullName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json");               
                
                var jsonStr = File.ReadAllText(configFileFullName);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<LocalConfig>(jsonStr);
            }


            private void GetTestResult(NUnit.Core.TestResult result, ref  TestRail.TestResult tc,ref string failureTestCases)
            {
                string[] featuretags = FeatureContext.Current.FeatureInfo.Tags;
                string[] tagstr = ScenarioContext.Current.ScenarioInfo.Tags;
                if (result.ResultState == ResultState.Ignored) return;                
                string result_status = string.Empty;
                string result_message = string.Empty;
                string result_stackTrace = string.Empty;
                              
                tc = new TestRail.TestResult();              
                foreach (string str in tagstr)
                {
                    string tagname = str.Split(':')[0].ToUpper();
                    string tagvalue = str.Substring(str.IndexOf(str.Split(':')[1]));
                    switch (tagname)
                    {
                        case "CASEID": tc.Case_id = tagvalue; break;
                        case "PRIORITY": tc.Priority = tagvalue; break;
                        case "MILESTONE": tc.MileStone = tagvalue; break;
                        case "TYPE": tc.Type = tagvalue; break;
                        // case "SUITENAME": tc.Suite_Name = tagvalue; break;
                    }


                }              
                tc.Suite_Id = tru.GetSuiteIdByCaseId(tc.Case_id);               


                tc.Result_status = result.ResultState.ToString();
                tc.Result_message=output;               
                 if (!result.IsSuccess)
                    { 
                      //log failure test cases
                        failureTestCases+="caseid:"+tc.Case_id+",";                        

                    }
               
            }
                    
            public void AddResult(TestRail.TestResult trs)
            {
                tru.AddTestResult(trs);
            }





            public List<string> GetTestRunsByPlanNameAndSuiteId(string projectName, string planName, string suiteid, string caseid)
            {
                List<Run> runs = tru.GetRunIdsByProjectNameAndPlanNameAndSuiteId(projectName, planName, suiteid);
                List<string> ls_runids2 = new List<string>();
                string runId = string.Empty;
                foreach (Run run in runs)
                {
                    List<TestRail.Test> ls_ts = tru.GetTestsByRunId(run.Id);
                    TestRail.Test tt = ls_ts.Find(p => p.CaseId == caseid);
                    if (tt != null)
                    {
                        ls_runids2.Add(tt.RunId);
                    }
                }
                return ls_runids2;
            }

  }

    
}
