namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using Fanex.NAP.CTS.Resource.Consts;
    using System;
    using System.Globalization;

    public class MatchesAssociationDetailViewModel
    {
        public int MatchId { get; set; }

        public string EventDateString { get; set; }

        public DateTime? EventDate
        {
            get
            {
                if (!string.IsNullOrEmpty(EventDateString))
                {
                    DateTime.TryParseExact(EventDateString, Consts.FormatDateToString_yyyyMMddHHmmss, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime eventDate);

                    return eventDate;
                }

                return null;
            }
        }

        public Int64 CustID1 { get; set; }

        public Int64 CustID2 { get; set; }

        public bool IsHavingSGB { get; set; }

        public string BettypeIDs { get; set; }

        public bool IsHavingGGB { get; set; }

        public string States { get; set; }

        public string GroupIDs { get; set; }
    }
}