namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface ICustomerClassificationService
    {
        bool RetryPushCCToMainDB(IEnumerable<LatestClassificationModel> customerClassifications, int retryTime);

        void PushCustomerClassificationToSoon88(IEnumerable<LatestCCDangerLevelModel> customerClassifications);

        bool PushDataToExternalPackge(IEnumerable<long> custIds, PushDataToExternalModel paras, FromFlow fromFlow);

        bool PushDataToExternal(IEnumerable<long> custIdsToSyncCC, PushDataToExternalModel paras);

        IEnumerable<LatestClassificationModel> GetLatestClassification(string custIds);

        IEnumerable<LatestCCDangerLevelModel> GetLatestCCAndDangerLevel(IEnumerable<long> custIds);
    }
}
