using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebUI_New.PageObejctModel;

namespace WebUI_New.Utilities
{
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
    public class TestAttribute: TestMethodAttribute 
    {
        public ApplicationMode ApplicationMode;
    }
    public enum ApplicationMode
    {
        Normal,
        SuperAdmin,
        Admin,
        SysAdmin
    }
}
