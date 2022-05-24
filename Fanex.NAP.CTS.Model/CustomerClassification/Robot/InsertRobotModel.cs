namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using System;

    public class InsertRobotModel
    {
        public InsertRobotModel()
        {
            IsFromTW = false;
        }

        public Int64 CustID { get; set; }

        public int CategoryID { get; set; }

        public int? RobotCounter { get; set; }

        public decimal? TurnoverRM { get; set; }

        public decimal? WinlossRM { get; set; }

        public Int64? BetCount { get; set; }

        public int? ActiveDays { get; set; }

        //True: Robot from TW, False: Robot Classification
        public bool IsFromTW { get; set; }

        public Int64? CreatedBy { get; set; }
    }
}