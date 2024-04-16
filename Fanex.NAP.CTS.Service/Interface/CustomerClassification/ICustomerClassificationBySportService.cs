namespace Fanex.NAP.CTS.Service.Interface.CustomerClassification
{
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using System.Collections.Generic;

    public interface ICustomerClassificationBySportService
    {
        bool PushCCBySportToMainDB(IEnumerable<InsertNormalAccountCustInfoModel> custInfos, int batchExternalSize);
    }
}
