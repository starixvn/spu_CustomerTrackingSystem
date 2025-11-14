namespace Fanex.NAP.CTS.Service.Interface.CustomerClassificationBySport.ProblemAccountInsertBySport
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassificationBySport;
    using Fanex.NAP.CTS.Model.CustomerClassificationBySport.ProblemAccountInsertBySport;
    using System.Collections.Generic;

    public interface IProblemAccountBySportService
    {
        IEnumerable<ProblemAccountLosingBySportModel> GetPALosingBySport(IEnumerable<BaseCustomerInfoBySportModel> custInfos);

        IEnumerable<InsertPABySportResultModel> InsertProblemAccountBySport(IEnumerable<InsertProblemAccountBySportModel> paBySports, BySportCCInputFlow cCInputFlow);

        IEnumerable<InsertPABySportResultModel> RescanCustomerClassificationBySport(IEnumerable<RescanCustomerClassificationBySportModel> custRescans, BySportCCInputFlow ccInputFlow);
    }
}
