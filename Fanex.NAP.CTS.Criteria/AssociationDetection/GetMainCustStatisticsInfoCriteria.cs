namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetMainCustStatisticsInfoCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "listCustID")]
        public string CustIDList { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_CTS_GetCustStatisticsInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
