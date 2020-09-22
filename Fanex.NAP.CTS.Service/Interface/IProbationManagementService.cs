namespace Fanex.NAP.CTS.Service.Interface
{    
    using Fanex.NAP.CTS.Model.ProbationManagement;
    using System.Collections.Generic;

    public interface IProbationManagementService
    {
        IEnumerable<CategoryModel> GetCategories(int categoryMode);

        IEnumerable<ProbationCategoryModel> GetProbationCategory(string username, int dayLeft, int categoryMode);

        IEnumerable<AccumulatedInfoModel> GetAccummulatedInfo(IEnumerable<ProbationModel> probationModelList);

        ResultChangeCategoryToCts ChangeCategory(string jsonData, string remark, int createdBy);
    }
}
