using Fanex.NAP.CTS.Model.MatchMonitor.Entities;

namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    public class MMDetailFilterTicketInfoModel
    {
        public MMDetailFilterTicketInfoModel() { }

        public MMDetailFilterTicketInfoModel(MMDetailFilterTicketInfoEntity entity)
        {
            BetTeam = entity.BetTeam;
            LiveHomeScore = entity.LiveHomeScore;
            LiveAwayScore = entity.LiveAwayScore;
            IsLicensee = entity.IsLicensee;
            CustomerClass = entity.CustomerClass;
            Danger1 = entity.Danger1;
            Danger2 = entity.Danger2;
            Danger3 = entity.Danger3;
            Danger4 = entity.Danger4;
            Danger5 = entity.Danger5;
            CurrencyId = entity.CurrencyId;
            CurrencyName = entity.CurrencyName;
            SuspiciousReason = entity.SuspiciousReason;
            SuspiciousGroupId = entity.SuspiciousGroupId;
            Hdp = entity.Hdp;
            BetTeamFullName = entity.BetTeamFullName;
            TicketStatus = entity.TicketStatus;
            IsCashout = entity.IsCashout ? 1 : 0;
        }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        public bool IsLicensee { get; set; }

        public int CustomerClass { get; set; }

        public int Danger1 { get; set; }

        public int Danger2 { get; set; }

        public int Danger3 { get; set; }

        public int Danger4 { get; set; }

        public int Danger5 { get; set; }

        public short CurrencyId { get; set; }

        public string CurrencyName { get; set; }

        public string SuspiciousReason { get; set; }

        public int SuspiciousGroupId { get; set; }

        public decimal Hdp { get; set; }

        public string BetTeam { get; set; }

        public string BetTeamFullName { get; set; }

        public int TicketStatus { get; set; }

        public int IsCashout { get; set; }
    }
}
