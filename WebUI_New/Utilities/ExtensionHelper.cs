using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUI_New.Utilities
{
    public static class ExtensionHelper
    {
        public static IWebElement FilterElement(this IList<IWebElement> ListOfElement, string Text)
        {
            foreach (var item in ListOfElement)
            {
                if (item.Text == Text)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
