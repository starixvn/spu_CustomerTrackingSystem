namespace Fanex.NAP.CTS.Criteria.FraudCase.ScannedOfficeIP
{  
    using Fanex.NAP.CTS.Core.BaseClasses;

    public class OfficeIPInsertCriteria : CommandBase
    {
        public int UserId { get; set; }

        public string IPRange { get; set; }

        public int FromNumber { get; set; }

        public int ToNumber { get; set; }

        public override string GetSettingKey() => "FS_Tbl_OfficeIP_Insert";

        public override bool IsValid() => true;
    }
}