namespace Fanex.NAP.CTS.Service.Interface.CustomerClassificationBySport.ProblemAccountInsertBySport
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassificationBySport;
    using Fanex.NAP.CTS.Model.CustomerClassificationBySport.ProblemAccountInsertBySport;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface IProblemAccountBySportService
    {
        IEnumerable<ProblemAccountLosingBySportModel> GetPALosingBySport(IEnumerable<BaseCustomerInfoBySportModel> custInfos, BySport_FunctionId functionId);

        IEnumerable<InsertPABySportResultModel> InsertProblemAccountBySport(IEnumerable<InsertProblemAccountBySportModel> paBySports, BySportCCInputFlow cCInputFlow, BySport_FunctionId functionId);
    }
}
