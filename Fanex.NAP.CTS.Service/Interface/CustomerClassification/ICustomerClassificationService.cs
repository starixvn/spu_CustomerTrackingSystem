namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface ICustomerClassificationService
    {
        bool RetryPushCCToMainDB(IEnumerable<LatestClassificationModel> customerClassifications, int retryTime);
 
        bool PushDataToExternalPackge(IEnumerable<long> custIds, PushDataToExternalModel paras, FromFlow fromFlow);

        bool PushDataToExternal(IEnumerable<long> custIdsToSyncCC, PushDataToExternalModel paras);

        IEnumerable<LatestCCDangerLevelModel> GetLatestCCAndDangerLevel(IEnumerable<long> custIds);

        IEnumerable<LatestCCDangerLevelModel> GetLatestCCAndDangerLevel(string custIds);

        DBResult UpdateDangerLevelToMainDB(IEnumerable<DangerLevelModel> dangerLevel, int dangerType);
    }
}
