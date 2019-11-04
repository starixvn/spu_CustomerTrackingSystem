namespace Fanex.NAP.CTS.Site.Controllers
{
    using Fanex.NAP.Core.Web.Attribute;
    using System;

    [NapSuperAuth]
    public abstract class ReportsController : BaseController
    {
        public string GetExcelReportName(string report)
        {
            return $"{report}_{DateTime.Today.ToString("yyyyMMdd")}";
        }
    }
}
