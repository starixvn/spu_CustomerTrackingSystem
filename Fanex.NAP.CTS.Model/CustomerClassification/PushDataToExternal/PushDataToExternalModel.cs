namespace Fanex.NAP.CTS.Model.SyncCustomerToMainDb
{
    public class PushDataToExternalModel
    {        
        public int ExternalBatchSize { get; set; }

        public bool IsUpdateDangerToMainDB { get; set; }

        public bool IsSyncAFC { get; set; }
    }
}