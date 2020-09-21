namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.ProbationManagement;
    using Fanex.NAP.CTS.Model.SpecialCustomerClass;
    using System.Collections.Generic;

    public interface IProbationManagementService
    {
        IEnumerable<CategoryModel> GetCategories(int categoryMode);

        IEnumerable<ProbationCategoryModel> GetProbationCategory(string username, int dayLeft, int categoryMode);

        IEnumerable<AccumulatedInfoModel> GetAccummulatedInfo(IEnumerable<ProbationModel> probationModelList);

        ResultChangeCategoryToCts ChangeCategory(string jsonData, string remark, int createdBy);

        DBResult SyncCustomerClassToMainDb(IEnumerable<CustomerClassForMainDb> customerClassList);
    }
}
