using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;

namespace WebUI_New.PageObejctModel
{
    public class BasePageFindElement
    {
        public IWebDriver _driver;
        public IJavaScriptExecutor js;

        public BasePageFindElement(IWebDriver driver)
        {
            _driver = driver;
        }

        public IWebElement ElementById(String KeyName)
        {
            //String elementName = GetConfigurationValue(KeyName);
            WaitForElement(KeyName, "Id");
            Console.WriteLine("Element Found " + KeyName);
            return _driver.FindElement(By.Id(KeyName));

        }

        public IWebElement ElementByXpath(String Value)
        {
            //String elementName = GetConfigurationValue(KeyName);
            //WaitForElement(elementName, "Id");
            //Console.WriteLine("Element Found " + KeyName);
            return _driver.FindElement(By.XPath(Value));

        }

        public IWebElement ElementByCss(String KeyName, bool skipWait=false)
        {
            //String elementName = GetConfigurationValue(KeyName);
            if (!skipWait)
            {
                WaitForElement(KeyName, "CSS");
                Console.WriteLine("Element Found " + KeyName);
            }
            return _driver.FindElement(By.CssSelector(KeyName));

        }

       
        // Fetching element for Ilist
        public IList<IWebElement> ElementInIlist(string KeyName, bool skipWait = false)
        {
            IList<IWebElement> ListElement = new List<IWebElement>();
            // String elementName = GetConfigurationValue(KeyName);

            if (!skipWait)
            {
                WaitForElement(KeyName, "CSS");
            }
            else
            {
                return _driver.FindElements(By.CssSelector(KeyName));
            }

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));
            ListElement = wait.Until<IList<IWebElement>>((d) =>
            {
                IList<IWebElement> elements = d.FindElements(By.CssSelector(KeyName));
                if (elements.Count == 0)
                {
                    return null;
                }
                Console.WriteLine("List element found " + KeyName);
                return elements;
            });
            return ListElement;
        }

        public IWebElement GetElementFromListByText(string KeyName, string text, bool skipWait = false)
        {
            IList<IWebElement> List= ElementInIlist(KeyName,skipWait);
            foreach (var item in List)
            {
                string itemText = item.Text;


                if (itemText?.ToLower() == text?.ToLower())
                {
                    return item;
                }
            }

            return null;
        }

        //Get Configuration value from CSS
        public string GetConfigurationValue(string KeyName)
        {
            string elementValue = ConfigurationManager.AppSettings.Get(KeyName);
            return elementValue;
        }


        //Function for waiting element
        private void WaitForElement(String Element, String Locator)
        {
            var wait = new DefaultWait<IWebDriver>(_driver)
            {
                Timeout = TimeSpan.FromSeconds(90),
                PollingInterval = TimeSpan.FromMilliseconds(30000)
            };
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            if (Locator == "CSS")
                wait.Until(d => d.FindElement(By.CssSelector(Element)));
            else
                wait.Until(d => d.FindElement(By.Id(Element)));
        }

        public bool IsElementPresent(string KeyName)
        {
            try
            {
                _driver.FindElement(By.CssSelector(KeyName));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
