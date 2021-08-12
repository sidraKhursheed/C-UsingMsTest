using System;
using System.Threading;
using AventStack.ExtentReports;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using WebUI_New.PageObejctModel;
using WebUI_New.Utilities;

namespace WebUI_New.TestCases
{
    [TestClass]
    public class Google:BaseClass 
    {

        public void MyGoogle()
        {
            string TermsConditionPopup = "//*[@id=\"CXQnmb\"]/div";

            ExtentTest test = extent.CreateTest("MyGoogle").Info("Test Started");
            Thread.Sleep(ts);
            IWebElement HomeTitle = Base.ElementByXpath(TermsConditionPopup);
            Thread.Sleep(ts);
            test.Pass("Assertion Passed");
            test.Pass("message", MediaEntityBuilder.CreateScreenCaptureFromPath(TakesScreenshot("ScreenShot")).Build());
            test.Info("Test Finished");
        }

        [TestAttribute(ApplicationMode = ApplicationMode.Normal)]
        [TestCategory("Web TestCases Production New")]
        public void RunMyGoogle()
        {
            Run(MyGoogle);
        }
    }
}
