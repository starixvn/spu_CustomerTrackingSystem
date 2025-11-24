namespace Fanex.NAP.CTS.Service.Interface.CustomerClassificationBySport
{
    using Fanex.NAP.CTS.Model.CustomerClassificationBySport;
    using System.Collections.Generic;

    public interface ICustomerClassificationBySportService
    {
        bool PushCCBySportToMainDB(IEnumerable<BaseCustomerInfoBySportModel> custInfos, int batchExternalSize, int retryTime);
    }
}
