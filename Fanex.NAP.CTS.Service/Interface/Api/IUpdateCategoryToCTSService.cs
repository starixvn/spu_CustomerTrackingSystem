namespace Fanex.NAP.CTS.Service.Interface.Api
{
    using Fanex.NAP.CTS.Model.API.UpdateCategoryWhenPAUpdated;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using System.Collections.Generic;

    public interface IUpdateCategoryService
    {
        IEnumerable<UpdateCategoryModel> UpdateCategoryToCTSWhenPAUpdated(UpdateCategoryParamsModel model, IEnumerable<ProblemAccountLosingClassificationModel> paLosing);
    }
}
