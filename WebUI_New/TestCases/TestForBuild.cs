using System;
using System.Collections.Generic;
using System.Threading;
using AventStack.ExtentReports;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using WebUI_New.PageObejctModel;
using WebUI_New.Utilities;

namespace WebUI_New.TestCases
{
    [TestClass]
    public class TestForBuild : BaseClass
    {
        public void TestBuild()
        {
            Console.WriteLine("Testing Base Class");
        }

        [TestAttribute(ApplicationMode = ApplicationMode.Normal)]
        [TestCategory("Web TestCases Production New")]
        public void RunTestForBuild()
        {
            Run(TestBuild);
        }
    }  
}
