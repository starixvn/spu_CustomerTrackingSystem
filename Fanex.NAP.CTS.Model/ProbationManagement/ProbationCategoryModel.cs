using System;

namespace Fanex.NAP.CTS.Model.ProbationManagement
{
    public class ProbationCategoryModel
    {
        public int CustID { get; set; }

        public string Username { get; set; }

        public string Site { get; set; }

        public int SportGroupID { get; set; }

        public string Sport { get; set; }

        public string TargetCategory { get; set; }

        public DateTime TaggedDate { get; set; }

        public decimal FirstMargin { get; set; }

        public decimal CurrentMargin { get; set; }

        public int DaysLeft { get; set; }
        
    }
}
