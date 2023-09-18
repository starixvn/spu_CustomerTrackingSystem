namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    using Fanex.NAP.CTS.Model.MatchMonitor;

    public class DependOnFilterItemViewModel
    {
        public DependOnFilterItemViewModel(MMDetailFilterTicketInfoModel info, bool isLive)
        {
            this.Score = isLive ? $"{info.LiveHomeScore} - {info.LiveAwayScore}" : "-";
            this.SubscriberType = info.IsLicensee ? "L" : "C";
            this.CC = info.CustomerClass;
            this.Danger1 = info.Danger1;
            this.Danger2 = info.Danger2;
            this.Danger3 = info.Danger3;
            this.Danger4 = info.Danger4;
            this.Danger5 = info.Danger5;
            this.CurrencyId = info.CurrencyId;
            this.CurrencyName = info.CurrencyName;
            this.ReasonItem = info.SuspiciousReason ?? "-";
            this.ADetectGroup = info.SuspiciousGroupId == 0 ? "-" : info.SuspiciousGroupId.ToString();
            this.ADetectGroupID = info.SuspiciousGroupId;
            this.BallHeadHDP = info.Hdp;
            this.BallHeadBetTeam = info.BetTeam;
            this.TicketStatus = info.TicketStatus;
        }

        public string Score { get; set; }

        public string SubscriberType { get; set; }

        public int CC { get; set; }

        public int Danger1 { get; set; }

        public int Danger2 { get; set; }

        public int Danger3 { get; set; }

        public int Danger4 { get; set; }

        public int Danger5 { get; set; }

        public string DangerToString
        {
            get
            {
                var danger = string.Empty;
                var commaCount = 0;

                if (Danger1 != 0)
                {
                    danger += "OR" + Danger1;
                }

                if (Danger2 != 0)
                {
                    if (string.IsNullOrEmpty(danger))
                    {
                        danger += "A" + Danger2;
                    }
                    else
                    {
                        commaCount++;

                        danger += "," + AddSpaceDanger(commaCount) + "A" + Danger2;
                    }
                }

                if (Danger3 != 0)
                {
                    if (string.IsNullOrEmpty(danger))
                    {
                        danger += "OS" + Danger3;
                    }
                    else
                    {
                        commaCount++;

                        danger += "," + AddSpaceDanger(commaCount) + "OS" + Danger3;
                    }
                }

                if (Danger4 != 0)
                {
                    if (string.IsNullOrEmpty(danger))
                    {
                        danger += "B" + Danger4;
                    }
                    else
                    {
                        commaCount++;

                        danger += "," + AddSpaceDanger(commaCount) + "B" + Danger4;
                    }
                }

                if (Danger5 != 0)
                {
                    if (string.IsNullOrEmpty(danger))
                    {
                        danger += "E" + Danger5;
                    }
                    else
                    {
                        commaCount++;

                        danger += "," + AddSpaceDanger(commaCount) + "E" + Danger5;
                    }
                }

                return (danger == string.Empty) ? "-" : danger;
            }
        }

        public string CurrencyName { get; set; }

        public short CurrencyId { get; set; }

        public string ReasonItem { get; set; } = "-";

        public string ADetectGroup { get; set; } = "-";

        public int ADetectGroupID { get; set; }

        public decimal BallHeadHDP { get; set; }

        public string BallHeadBetTeam { get; set; }

        public int TicketStatus { get; set; }

        private string AddSpaceDanger(int commaCount)
        {
            return commaCount % 2 == 0 ? " " : string.Empty;
        }
    }
}
