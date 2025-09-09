namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using System;

    public class InsertProblemAccountModel : CustomerClassificationModel
    {
        public InsertProblemAccountModel()
        {
            IsFromTVS = false;
            IsFromCTS = false;
            IsFromTW = false;           
        }


        public InsertProblemAccountModel(RescanCustomerClassificationModel data)
        {
            CTSCustID = data.CTSCustID;
            CustID = data.CustID;
            CategoryID = data.CategoryID;
            TVSReasonID = data.TVSReasonID;
            RobotCounter = data.RobotCounter;
            TVSRequestID = data.TVSRequestID;
            IsParlay = data.IsParlay;
            IssueTypeID = data.IssueTypeID;
            SportType = data.SportType;
            DWSportType = data.DWSportType;
            RoleID = data.RoleID;
            SubscriberID = data.SubscriberID;
            IsLicensee = data.IsLicensee;
            Remark = data.Remark;
            CreatedBy = data.CreatedBy;
            IsMarkedDirectly = data.IsMarkedDirectly;
            IsFromTVS = data.IsFromTVS;
            IsFromTW = data.IsFromTW;
            IsFromCTS = data.IsFromCTS;
            IsFromAI = data.IsFromAI;
            IsFromImperva = data.IsFromImperva;
            LastModifiedDate = data.LastModifiedDate;
            CreatedDate = data.CreatedDate;
        }     

        public long CTSCustID { get; set; }

        public int RoleID { get; set; }

        public int SubscriberID { get; set; }

        public bool? IsLicensee { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string Remark { get; set; }

        public Int64? CreatedBy { get; set; }

        /* Data warehouse, mainDB  info */

        public int? WinlossStatus { get; set; } // LOSING  = 0, KEEPSTATE = 1, WINNING = 2      

        public int? TVSReasonID { get; set; }

        public bool? IsMarkedDirectly { get; set; }

        public bool IsFromAI { get; set; }

        public bool IsFromTVS { get; set; }

        public int? TVSRequestID { get; set; }

        public bool? IsParlay { get; set; }

        public int? SportType { get; set; }

        public int DWSportType { get; set; }

        public int? IssueTypeID { get; set; }

        public int? RobotCounter { get; set; }

        public bool IsFromTW { get; set; }

        public bool IsFromCTS { get; set; }

        public bool IsFromImperva { get; set; }

        public DateTime? CreatedDate { get; set; }

    }
}