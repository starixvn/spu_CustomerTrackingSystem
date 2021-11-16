namespace Fanex.NAP.CTS.Model.API.ProblemCaseMonitor
{
    using Fanex.NAP.CTS.Model.API.UpdateCategoryWhenPAUpdated;
    using System.Collections.Generic;

    public class UpdateCategoryParamsModel
    {
        public IEnumerable<CustInfoModel> CustInfos { get; set; }

        public int UserId { get; set; }

    }
}
