namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustStatisticsInfoRealtimeCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ListCustID")]
        public string CustIDList { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_GetCustStatisticsInfo_Realtime_ByListCustID";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
