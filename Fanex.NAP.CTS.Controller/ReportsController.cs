namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using System;

    [NapSuperAuth]
    public class ReportsController : BaseController
    {
        public string GetExcelReportName(string report)
        {
            return $"{report}_{DateTime.Today.ToString("yyyyMMdd")}";
        }
    }
}
