namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using System.Collections.Generic;

    public interface ICustomerClassificationService
    {
        bool RetryPushCCToMainDB(IEnumerable<LatestClassificationModel> customerClassifications, int retryTime);

        void PushCustomerClassificationToSoon88(IEnumerable<LatestCCDangerLevelModel> customerClassifications);

        bool PushDataToExternal(IEnumerable<long> custIdsToSyncCC, PushDataToExternalModel paras);

        IEnumerable<LatestClassificationModel> GetLatestClassification(string custIds);
    }
}
