namespace Fanex.NAP.CTS.Service.Interface.CustomerClassification
{
    using Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface IRescanCustomerClassificationBySportService
    {
        void ResanNormalBySportForNewCustomers(IEnumerable<RescanNormalBySportListCustModel> custIds, BySport_FunctionId functionId);
    }
}
