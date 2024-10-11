namespace Fanex.NAP.CTS.Model.SyncCustomerToMainDb
{
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class PushDataToExternalModel
    {        
        public int ExternalBatchSize { get; set; }

        public bool IsUpdateDangerToMainDB { get; set; }

        public bool IsSyncAFC { get; set; }

        public RoleGroupType RoleGroup { get; set; }
    }
}