namespace Fanex.NAP.CTS.Criteria.FraudCase.ScannedOfficeIP
{   
    using Fanex.NAP.CTS.Core.BaseClasses;

    public class OfficeIPDeleteCriteria : CommandBase
    {
        public int UserId { get; set; }

        public string IPRange { get; set; }

        public int FromNumber { get; set; }

        public int ToNumber { get; set; }

        public override string GetSettingKey() => "FS_Tbl_OfficeIP_Delete";

        public override bool IsValid() => true;
    }
}