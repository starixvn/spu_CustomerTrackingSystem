namespace Fanex.NAP.CTS.Model.API.UpdateCategoryWhenPAUpdated
{
    using System;

    public class UpdatePALosingInfoModel
    {
        public long CustID { get; set; }

        public int? FromReasonID { get; set; }

        public int? ToReasonID { get; set; }

        public int? WinlossStatus { get; set; }

        public decimal? TurnoverRM { get; set; }

        public decimal? WinlossRM { get; set; }

        public Int64? BetCount { get; set; }

        public int? ActiveDays { get; set; }
    }
}
