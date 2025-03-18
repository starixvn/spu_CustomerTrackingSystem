namespace Fanex.NAP.CTS.Criteria.Common
{
    using Fanex.Data.Repository;

    public class GetCommonSportBettypeCriteria : CriteriaBase
    {
        public int ProductId { get; set; }

        public string SportBettypeList { get; set; }

        public override string GetSettingKey() => "CTS_SportBettype_Get";

        public override bool IsValid() => true;
    }
}