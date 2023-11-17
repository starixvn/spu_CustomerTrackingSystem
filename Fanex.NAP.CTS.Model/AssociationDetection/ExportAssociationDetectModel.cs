namespace Fanex.NAP.CTS.Model.AssociationDetection
{
    using System;

    public class ExportAssociationDetectModel
    {
        public ExportAssociationDetectModel() { }       

        public string UserName { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CustomerClass { get; set; }

        public int? GroupID { get; set; }

        public int Danger1 { get; set; }

        public int Danger2 { get; set; }

        public int Danger3 { get; set; }

        public int Danger4 { get; set; }

        public int Danger5 { get; set; }

        public int? TotalSettledTickets { get; set; }

        public string CustomerCategory { get; set; }

        public decimal? TotalWinloss { get; set; }

        public decimal? WinlossLast3Days { get; set; }

        public decimal? WinlossLast7Days { get; set; }

        public decimal? TotalSabaTurnoverRM { get; set; }

        public decimal? SabaTurnoverRMLast3Days { get; set; }

        public decimal? SabaTurnoverRMLast7Days { get; set; }

        public decimal? TotalTurnover { get; set; }

        public decimal? TurnoverLast3Days { get; set; }

        public decimal? TurnoverLast7Days { get; set; }

        public int BotSumTrans { get; set; }

        public double BotPercentageTrans { get; set; }

        public int InvalidBrowserInfoSumTrans { get; set; }

        public double InvalidBrowserInfoPercentageTrans { get; set; }

        public string DangerLevel 
        {
            get
            {
                var danger = string.Empty;

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
                        danger += ", A" + Danger2;
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
                        danger += ", OS" + Danger3;
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
                        danger += ", B" + Danger4;
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
                        danger += ", E" + Danger5;
                    }
                }

                return (danger == string.Empty) ? "" : danger;
            }
        }
    }
}
