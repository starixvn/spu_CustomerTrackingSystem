namespace Fanex.NAP.CTS.Model.Account
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using System;

    public class CustomerClassificationHistoryModel
    {
        public long ID { get; set; }

        public DateTime CreatedTime { get; set; }

        public string CategoryName { get; set; }

        public string CustomerClass { get; set; }

        public int CategoryID { get; set; }

        public string SourceTypeName { get; set; }

        public int SourceTypeID { get; set; }

        public decimal TurnOver { get; set; }

        public decimal WinLoss { get; set; }

        public decimal Margin { get; set; }

        public int BetCount { get; set; }

        public int ActiveDays { get; set; }

        public int RobotCounter { get; set; }

        public string CreatedBy { get; set; }

        public long TWBetCount { get; set; }

        public short? First5TWGBTicketCount { get; set; }

        public decimal TWGroupBettingRate { get; set; }

        public decimal TWTicketRejectRate { get; set; }

        public decimal TWDesktopUsageRate { get; set; }

        public string Remark { get; set; }

        public int TVSRequestID { get; set; }

        public bool IsFromTVS { get; set; }

        public bool IsFromTW { get; set; }

        public bool IsFromCTS { get; set; }

        public bool IsFromAI { get; set; }

        public bool IsFromImperva { get; set; }

        public string LicVIP { get; set; }

        public string LicBA { get; set; }  

        public bool? IsParlay { get; set; }

        public int ? SportType { get; set; }

        public int IssueTypeID { get; set; }

        public string SportName { 
            get 
            {
                if (this.SportType != null)
                {
                    return CachedRef.GetSportTypeName(this.SportType.Value);
                }

                return string.Empty;
            } 
        }

        public bool IsGetSportName { get; set; }
    }
}
