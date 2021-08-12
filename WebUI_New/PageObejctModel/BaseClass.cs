using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using WebUI_New.Utilities;
using EdgeDriver = Microsoft.Edge.SeleniumTools.EdgeDriver;
using EdgeOptions = Microsoft.Edge.SeleniumTools.EdgeOptions;

namespace WebUI_New.PageObejctModel
{

    public class BaseClass
    {
        public IWebDriver _driver;
        public TimeSpan ts = TimeSpan.FromSeconds(03);
        public BasePageFindElement Base;
        public ExtentReports extent = null;
        public bool isIndependent = true;
        

        //Calling the parameter from runSetting file
        public TestContext TestContext { get; set; }

        public BaseClass(bool _isIndependent = true)
        {
            isIndependent = _isIndependent;           
                      
        }
        private void InvokeMethodWithConfig(string configPath, Action TestMethod)
        {
            var directory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            var settingsPath = Path.Combine(directory, configPath);
            var settings = File.ReadAllText(settingsPath);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(settings);
            foreach (XmlNode row in doc.SelectNodes("//TestRunParameters//Parameter"))
            {
                var rowName = row.Attributes["name"].Value;
                var rowValue = row.Attributes["value"].Value;
                if (TestContext.Properties.Contains(rowName))
                {
                    TestContext.Properties.Remove(rowName);
                }
                TestContext.Properties.Add(rowName, rowValue);
            }
            try
            {
                TestInit();
                TestMethod.Invoke();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                TestClean();
            }
        }
        protected string BrowserName;
        protected string BoxName;
        protected string EnvName;
        protected string DBType;
        protected string UserType;

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        protected void Run(Action TestMethod)
        {
            var methodBase = (new System.Diagnostics.StackTrace())?.GetFrame(1)?.GetMethod();
            var UserMode = (methodBase.GetCustomAttributes(typeof(TestAttribute), true).FirstOrDefault() as TestAttribute)?.ApplicationMode;
            
            if (UserMode==null)
            {
                throw new Exception("Mode Not Defined in TestAttribute");
            }
            Console.WriteLine("Start Running " + methodBase.Name);
            var SelectedMode = ConfigurationManager.AppSettings["SelectedMode"];
            var SelectedBoxes = ConfigurationManager.AppSettings[SelectedMode + "_SelectedBoxes"]?.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries)?.Select(e=>e.ToLowerInvariant().Trim())?.ToList();
            var SelectedEnvs = ConfigurationManager.AppSettings[SelectedMode + "_SelectedEnvs"]?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)?.Select(e => e.ToLowerInvariant().Trim())?.ToList();
            var SelectedDBTypes = ConfigurationManager.AppSettings[SelectedMode + "_SelectedDBType"]?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)?.Select(e => e.ToLowerInvariant().Trim())?.ToList();
            var SelectedBrowsers = ConfigurationManager.AppSettings[SelectedMode + "_SelectedBrowsers"]?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)?.Select(e => e.ToLowerInvariant().Trim())?.ToList();
            foreach (var SelectedBrowser in SelectedBrowsers)
            {
                BrowserName = SelectedBrowser;
                
                foreach (var SelectedEnv in SelectedEnvs)
                {
                    EnvName = SelectedEnv;

                    string relativeFilePath;
                    switch (UserMode)
                    {
                        case ApplicationMode.Normal:
                            foreach (var SelectedBox in SelectedBoxes)
                            {
                                BoxName = SelectedBox;
                                UserType = "Normal";
                                foreach (var SelectedDB in SelectedDBTypes)
                                {
                                    DBType = SelectedDB; 
                                    relativeFilePath = "RunSettingFiles\\" + SelectedBox + "-" + SelectedDB + "-" + SelectedEnv + ".runsettings";
                                    InvokeMethodWithConfig(relativeFilePath, TestMethod);
                                }
                            }
                            break;
                        case ApplicationMode.Admin:
                            
                            break;
                        case ApplicationMode.SuperAdmin:
                            UserType = "SuperAdmin";
                            relativeFilePath = "RunSettingFiles\\SuperAdminFile-" + SelectedEnv + ".runsettings";
                            InvokeMethodWithConfig(relativeFilePath, TestMethod);
                            
                            break;
                        case ApplicationMode.SysAdmin:
                            throw new Exception("Config not found");
                            break;
                        default:
                            throw new Exception("Config not found");
                            break;
                    }
                }
            }

        }

        private void RunInChrome(bool isHeadless = false)
        {
         
            ChromeOptions options = new ChromeOptions();
            if (File.Exists("C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe"))
            {

                options.BinaryLocation = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
            }
            else
            {
                options.BinaryLocation = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
            }
            if (isHeadless)
            {
                options.AddArgument("--headless");
            }
            options.AddArgument("--no-sandbox");
            options.AddArgument("no-sandbox");
           
            _driver = new ChromeDriver(".", options, TimeSpan.FromMinutes(3));
            _driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(30));
            
        }
        private void RunInIE(bool isHeadless = false)
        {
            InternetExplorerOptions options = new InternetExplorerOptions();
            
            _driver = new InternetExplorerDriver(".",options);

        }

        private void RunInFirefox(bool isHeadless = false)
        {
            FirefoxOptions options = new FirefoxOptions();
            if (isHeadless)
            {
                options.AddArgument("--headless");
            }

            _driver = new FirefoxDriver(options);// InternetExplorerDriver(options);

        }

        private void RunInEdge(bool isHeadless = false)
        {
            // https://github.com/microsoft/edge-selenium-tools   ; For setting Edge Driver

            var options = new EdgeOptions();
            if (isHeadless)
            {
                options.AddArgument("--headless");
            }
            options.UseChromium = true;
            _driver = new EdgeDriver(options);

        }


        const string chrome = "chrome";
        const string firefox = "firefox";
        const string edge = "edge";
        const string ie = "ie";
        //[TestInitialize]
        public void TestInit()
        {
            switch (BrowserName)
            {
                case chrome:
                    RunInChrome();
                    break;
                case firefox:
                    RunInFirefox();
                    break;
                case edge:
                    RunInEdge();
                    break;
                case ie:
                    RunInIE();//it has some issues
                    break;
                default:
                    throw new Exception("invalid Browser Type " + BrowserName);
                    break;
            }

             Base = new BasePageFindElement(_driver);
            Console.WriteLine("Intialization test");
            _driver.Manage().Window.Maximize();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(200);
            string myURL = TestContext.Properties["URL"].ToString();
            _driver.Navigate().GoToUrl(myURL);
            Console.WriteLine("Url opened");
            ExtentStart();
            //Authenticate();
         }

        //[TestCleanup]
        public void TestClean()
        {
            Console.WriteLine("Cleaning Test");
            for (int driverDisposeTries = 0; driverDisposeTries < 3; driverDisposeTries++)
            {
                try
                {
                    //_driver.Close();
                    _driver.Dispose();
                    driverDisposeTries = 5;
                }
                catch (Exception)
                {
                    Console.WriteLine("Retrying Cleaning Test");
                }
            }
            try
            {
                if (_driver!=null)
                {

                    Console.WriteLine("Forcefully disposing Driver");
                    _driver.Dispose();
                }
            }
            catch (Exception)
            {
            }
            ExtentClose();
            Console.WriteLine("Closing the Browser");
        }

        
        public void Authenticate()
        {
            
            string UserName = "username";
            string Password = "password";
            string AuthenticateButton = "#loginForm > div> input.m-form-button";

            string usernamevalue = TestContext.Properties["UserName"].ToString();
            IWebElement UsernameElement = Base.ElementById(UserName);
            UsernameElement.SendKeys(usernamevalue);
            Console.WriteLine("User-name entered");
            string passwordvalue = TestContext.Properties["Password"].ToString();
            IWebElement PasswordElement = Base.ElementById(Password);
            PasswordElement.SendKeys(passwordvalue);
            Console.WriteLine("Password entered");
            IWebElement AutButton = Base.ElementByCss(AuthenticateButton);
            AutButton.Click();
            Console.WriteLine("Authenticate button clicked");
        }

        public void ExtentStart()
        {
            string pathProject = AppDomain.CurrentDomain.BaseDirectory;
            string pathScreen = pathProject.Replace("\\bin\\Debug", "");
            string path = pathScreen + "\\MyWebReports\\";

            Directory.CreateDirectory(path);
            string ReportName = Path.Combine(path, "ExtentReport" + "_");

            extent = new ExtentReports();
            string EnvConfigration = UserType + "_" + EnvName + "_" + BoxName + "_" + DBType + "_" + BrowserName;
            var htmlReporter = new ExtentHtmlReporter(ReportName + EnvConfigration.ToUpper() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".html");

            //htmlReporter.AppendExisting = true;
            extent.AttachReporter(htmlReporter);

            string hostname = Dns.GetHostName();
            OperatingSystem os = Environment.OSVersion;

            extent.AddSystemInfo("Operating System", os.ToString());
            extent.AddSystemInfo("HostName", hostname);
            extent.AddSystemInfo("Environment", "Production");
            extent.AddSystemInfo("Device", "Google Nexus Device");
        }

        public void ExtentClose()
        {
            extent.Flush();
        }

        public string TakesScreenshot(string FileName)
        {

            string pathProject = AppDomain.CurrentDomain.BaseDirectory;
            string pathScreen = pathProject.Replace("\\bin\\Debug", "");
            string path = pathScreen + "" + "//WebUI_NewScreenShots//";
           
            StringBuilder TimeAndDate = new StringBuilder(DateTime.Now.ToString());
            TimeAndDate.Replace("/", "_");
            TimeAndDate.Replace(":", "_");
            TimeAndDate.Replace(" ", "_");

            string imageName = FileName + TimeAndDate.ToString();

            Directory.CreateDirectory(path);
            string imageFileName = Path.Combine(path, imageName + "." + System.Drawing.Imaging.ImageFormat.Jpeg);

            ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile(imageFileName);
            return imageFileName;
        }
    }
}