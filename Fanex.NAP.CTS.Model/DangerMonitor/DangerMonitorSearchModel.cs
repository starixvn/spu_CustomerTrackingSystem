namespace Fanex.NAP.CTS.Model.DangerMonitor
{
    using Fanex.NAP.CTS.Core.Extentions;
    using System;
    using System.Collections.Generic;

    public class DangerMonitorSearchModel
    {

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string QueryDanger { get; set; }

        public string UserNameList { get; set; }

        public string LeagueGroupList { get; set; }

        public IEnumerable<string> SportList { get; set; }

        public string Event { get; set; } /* League / Match */

        public string Market { get; set; }

        public string MatchSection { get; set; }

        public string BetTypes { get; set; }

        public decimal UserAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public DangerMonitorUserRootOptionModel RootUserOption { get; set; }

        public bool IsRootUserAssociationMode
        {
            get
            {
                return RootUserOption != null && (RootUserOption.IsAI || RootUserOption.IsDevice || RootUserOption.IsIP);
            }
        }

        public bool Validate
        {
            get
            {
                if (FromDate != null && ToDate != null && FromDate > ToDate)
                {
                    return false;
                }

                if ((QueryDanger.IsNullOrEmpty() && UserNameList.IsNullOrEmpty()))
                {
                    return false;
                }

                if (Market.IsNullOrEmpty())
                {
                    return false;
                }

                if (MatchSection.IsNullOrEmpty())
                {
                    return false;
                }

                if (LeagueGroupList.IsNullOrEmpty())
                {
                    return false;
                }

                if (SportList.IsNullOrEmpty())
                {
                    return false;
                }

                if (BetTypes.IsNullOrEmpty() || BetTypes.Equals("[]"))
                {
                    return false;
                }

                if (UserAmount < 0 || TotalAmount < 0)
                {
                    return false;
                }

                if (RootUserOption == null)
                {
                    return false;
                }

                return true;
            }
        }
    }
}