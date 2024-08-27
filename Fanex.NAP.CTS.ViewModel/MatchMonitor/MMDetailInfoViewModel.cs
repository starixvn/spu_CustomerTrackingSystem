namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Model;
    using System;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class MMDetailInfoViewModel
    {
        public MMDetailInfoViewModel() { }

        private MMDetailInfoViewModel(MMDetailInfoViewModel para)
        {
            this.IsSupportShowAllTickets = para.IsSupportShowAllTickets;
            this.TypeOfTickets = para.TypeOfTickets;
            this.LastScannedSequenceID = para.LastScannedSequenceID;
            this.LastTransDate = para.LastTransDate;
            this.EventDateToString = para.EventDateToString;
            this.MatchID = para.MatchID;
            this.BetTypeID = para.BetTypeID;
            this.BetID = para.BetID;
            this.BetChoiceType = para.BetChoiceType;
            this.BetChoiceHome = para.BetChoiceHome;
            this.BetChoiceAway = para.BetChoiceAway;
            this.Sites = para.Sites;
            this.LiveIndicator = para.LiveIndicator;
            this.IsImportNewTicket = para.IsImportNewTicket;
            this.Sport = para.Sport;
            this.TranstimeFrom = para.TranstimeFrom;
            this.TranstimeTo = para.TranstimeTo;
            this.IsCashout = para.IsCashout;
        }

        public bool IsSupportShowAllTickets { get; set; }

        public MMViewModeFromUI? TypeOfTickets { get; set; }

        public long? LastScannedSequenceID { get; set; }

        public string LastTransDate { get; set; }

        public string EventDateToString { get; set; }

        public Int64 MatchID { get; set; }

        public int BetTypeID { get; set; }

        public Int64 BetID { get; set; }

        public int BetChoiceType { get; set; }

        public string BetChoiceHome { get; set; }

        public string BetChoiceAway { get; set; }

        public string Sites { get; set; }

        public bool LiveIndicator { get; set; }

        public bool IsImportNewTicket { get; set; }

        public int Sport { get; set; }

        public int? IsCashout { get; set; }

        public DateTime? TranstimeFrom { get; set; }

        public DateTime? TranstimeTo { get; set; }

        public DateTime EventDate
        {
            get
            {
                var eventDate = DateTime.MinValue;

                if (!string.IsNullOrEmpty(EventDateToString))
                {
                    return DateTime.TryParse(EventDateToString, out eventDate) ? eventDate : DateTime.MinValue;
                }

                return eventDate;
            }
        }

        public TicketType TicketType { get; set; }

        public MMDetailInfoViewModel Clone() => new MMDetailInfoViewModel(this);

        public string BuildCacheKey(string nameOfFuntion, string mmDetailRegion, string userID, string viewMode)
        {
            var transtimeFrom = TranstimeFrom?.Subtract(DateTime.MinValue).TotalSeconds.ToString() ?? string.Empty;
            var transtimeTo = TranstimeTo?.Subtract(DateTime.MinValue).TotalSeconds.ToString() ?? string.Empty;
            return $"{nameOfFuntion}_{mmDetailRegion}_{userID}_{MatchID}_{LiveIndicator}_{BetTypeID}_{BetID}_{viewMode}_{transtimeFrom}_{transtimeTo}";
        }

        public void SetLastMark(TicketDetailViewModel ticketDetail)
        {
            LastScannedSequenceID = ticketDetail.MinSequenceId;
            LastTransDate = ticketDetail.MinTransDate?.ToString(FormatDatetimeToString_YYYYmmddHHmmssfff);
        }
    }
}