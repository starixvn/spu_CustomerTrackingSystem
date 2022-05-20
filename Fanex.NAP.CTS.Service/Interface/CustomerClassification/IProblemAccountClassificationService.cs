namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using System.Collections.Generic;

    public interface IProblemAccountClassificationService
    {
        bool InsertProblemAccount(IEnumerable<InsertProblemAccountModel> problemAccounts, MappingCategoryByType mappingCategoryByType);

        ProblemAccountInsertResultModel InsertProblemAccount(IEnumerable<InsertProblemAccountModel> problemAccounts, IEnumerable<ProblemAccountLosingClassificationModel> probAccountLosing, MappingCategoryByType mappingCategoryByType);
       
    }
}
