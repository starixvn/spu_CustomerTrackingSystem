namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using System.Collections.Generic;

    public interface IPushDataToExternalService
    {
        bool RetryPushCCToMainDB(IEnumerable<LatestClassificationModel> customerClassifications, int retryTime);

        void PushCustomerClassificationToSoon88(IEnumerable<LatestClassificationModel> customerClassifications);

        bool PushDataToExternal(IEnumerable<long> custIds, PushDataToExternalModel paras);
    }
}
