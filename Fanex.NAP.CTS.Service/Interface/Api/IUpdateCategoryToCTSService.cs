namespace Fanex.NAP.CTS.Service.Interface.Api
{
    using Fanex.NAP.CTS.Model.API.ProblemCaseMonitor;
    using Fanex.NAP.CTS.Model.API.UpdateCategoryWhenPAUpdated;
    using System.Collections.Generic;

    public interface IUpdateCategoryService
    {
        IEnumerable<UpdateCategoryModel> UpdateCategoryToCTSWhenPAUpdated(UpdateCategoryParamsModel model);

    }
}
