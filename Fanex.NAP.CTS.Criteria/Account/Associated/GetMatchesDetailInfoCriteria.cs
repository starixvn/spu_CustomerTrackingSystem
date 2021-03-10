namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data.Repository;
    using System;

    public class GetMatchesDetailInfoCriteria : CriteriaBase
    {
        public Int64 CustId1 { get; set; }

        public Int64 CustId2 { get; set; }

        public DateTime EventDate { get; set; }

        public int MatchId { get; set; }

        public string Bettypes { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Rpt_BettingPattern_TicketDetails";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
