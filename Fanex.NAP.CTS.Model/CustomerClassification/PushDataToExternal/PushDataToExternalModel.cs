namespace Fanex.NAP.CTS.Model.SyncCustomerToMainDb
{
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class PushDataToExternalModel
    {
        public bool IsPushCCToSoon88 { get; set; }

        public int ExternalBatchSize { get; set; }

        public bool IsUpdateDangerToMainDB { get; set; }
    }
}