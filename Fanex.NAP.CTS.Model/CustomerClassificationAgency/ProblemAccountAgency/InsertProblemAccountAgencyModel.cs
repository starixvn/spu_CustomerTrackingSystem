namespace Fanex.NAP.CTS.Model.CustomerClassificationAgency.ProblemAccountAgency
{
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.BaseCustomerClassificationAgency;
    using System;

    public class InsertProblemAccountAgencyModel
    {
        public InsertProblemAccountAgencyModel(InsertProblemAccountModel data) 
        {
            CustID = data.CustID;
            CTSCustID = data.CTSCustID;
            RoleID = data.RoleID;
            SubscriberID = data.SubscriberID;
            CategoryID = data.CategoryID;
            CategoryGroupID = data.CategoryGroupID;
            IsLicensee = data.IsLicensee;
            CreatedBy = data.CreatedBy;
            Remark = data.Remark;
            IsMarkedDirectly = data.IsMarkedDirectly;
            WinlossStatus = data.WinlossStatus;
            TurnoverRM = data.TurnoverRM;
            WinlossRM = data.WinlossRM;
            BetCount = data.BetCount;
            IsFromCTS = data.IsFromCTS;
            CreatedDate = data.CreatedDate;
            PerformanceTime = data.PerformanceTime;
            IsFromTW = data.IsFromTW;
            IsFromAI = data.IsFromAI;
        }

        public InsertProblemAccountAgencyModel(RescanCustomerClassificationAgencyModel data) 
        {
            CTSCustID = data.CTSCustID;
            CustID = data.CustID;
            CategoryID = data.CategoryID;
            RoleID = data.RoleID;
            SubscriberID = data.SubscriberID;
            IsLicensee = data.IsLicensee;
            Remark = data.Remark;
            CreatedBy = data.CreatedBy;
            IsMarkedDirectly = data.IsMarkedDirectly;
            IsFromCTS = data.IsFromCTS;
            CreatedDate = data.CreatedDate;
        }      

        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public int? CategoryID { get; set; }

        public int? CategoryGroupID { get; set; }

        public bool? IsLicensee { get; set; }

        public Int64? CreatedBy { get; set; }

        public string Remark { get; set; }

        public bool? IsMarkedDirectly { get; set; }

        public int? WinlossStatus { get; set; } // LOSING  = 0, KEEPSTATE = 1, WINNING = 2

        public decimal? TurnoverRM { get; set; }

        public decimal? WinlossRM { get; set; }

        public Int64? BetCount { get; set; }

        public bool IsFromCTS { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? PerformanceTime { get; set; }

        public bool IsFromTW { get; set; }

        public bool IsFromAI { get; set; }
   
    }
}
