namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.ProbationManagement;
    using Fanex.NAP.CTS.ViewModel.ProbationManagement;
    using System.Collections.Generic;

    public interface IProbationManagementService
    {
        List<AccountStaticStatusModel> GetAccountStaticStatuses();        

        IEnumerable<ProbationCategoryModel> GetProbationCategory(string username, int dayLeft, int categoryMode);

        IEnumerable<ProbationCategoryModel> GetProbationCategory(ExportProbationCategoryFilter filter, out DBResult dBResult);

        IEnumerable<AccumulatedInfoModel> GetAccummulatedInfo(string customersXML);

        ResultChangeCategoryToCts ChangeCategory(string jsonData, string remark, int createdBy);

        string ExportCSV(IEnumerable<ProbationCategoryViewModel> probationCategoryList, ExportProbationCategoryFilter filter);

        byte[] ExportExcel(IEnumerable<ProbationCategoryViewModel> probationCategoryList, ExportProbationCategoryFilter filter, string template);
    }
}
