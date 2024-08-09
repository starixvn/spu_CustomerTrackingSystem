namespace Fanex.NAP.CTS.ViewModel.Account.CustomerCategoryBySport
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account.CustomerCategoryBySport;
    using Fanex.NAP.CTS.Resource.Consts;
    using System;

    public class GetCustomerCategoryHistoryBySportViewModel
    {
        public GetCustomerCategoryHistoryBySportViewModel(GetCustomerCategoryHistoryBySportModel data)
        {
            //this.ID = data.ID;
            this.CustID = data.CustID;
            this.CTSCustID = data.CTSCustID;
            this.SportID = data.SportID;
            this.CategoryID = data.CategoryID;
            this.CategoryName = data.CategoryName;
            //this.TurnoverRM = data.TurnoverRM;
            //this.WinlossRM = data.WinlossRM;
            //this.BetCount = data.BetCount;
            //this.ActiveDays = data.ActiveDays;            
            this.CustomerClass = (data.TargetCC == -1 || data.TargetCC == 0) ? "-" : data.TargetCC.ToString();
            this.CreatedTimeString = data.LastModifiedDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss);
            this.CreatedBy = data.CreatedBy;
            //this.SourceTypeID = data.SourceTypeID;
            //this.SourceTypeName = data.SourceTypeName;
            //this.Remark = data.Remark;
            //this.Reason = GetReason();
            this.Reason = data.Remark;
        }

        public long ID { get; set; }

        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int SportID { get; set; }

        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public decimal TurnoverRM { get; set; }

        public decimal WinlossRM { get; set; }

        public long BetCount { get; set; }

        public int ActiveDays { get; set; }

        public string CustomerClass { get; set; }

        public string CreatedTimeString { get; set; }

        public string Reason { get; set; }

        public string CreatedBy { get; set; }

        public string Remark { get; set; }

        public int SourceTypeID { get; set; }

        public string SourceTypeName { get; set; }

        //private string GetReason()
        //{
        //    var spanReason = MappingReasonByCategoryID();

        //    if (string.IsNullOrEmpty(spanReason))
        //    {
        //        spanReason = MappingReasonBySourceTypeID();
        //    }

        //    return spanReason;
        //}

        //private string MappingReasonBySourceTypeID()
        //{
        //    var result = string.Empty;
        //    switch (SourceTypeID)
        //    {
        //        case (int)Model.SourceTypeID.SpecialCustomerClass_Add:
        //        case (int)Model.SourceTypeID.SpecialCustomerClass_Remove:
        //            {
        //                result += $"<span> {SourceTypeName} </span>";

        //                if (!string.IsNullOrEmpty(Remark))
        //                    result += $"| {Remark} ";
        //                break;
        //            }
        //    }

        //    return result;
        //}

        //private string MappingReasonByCategoryID()
        //{
        //    var spanReason = "";
        //    var winlost = Math.Round(this.WinlossRM, 3);
        //    var turnOver = Math.Round(this.TurnoverRM, 3);
        //    var margin = new decimal();
        //    if (turnOver != 0)
        //    {
        //        margin = Math.Round((this.WinlossRM / this.TurnoverRM) * 100, 3);
        //    }
        //    var betCount = this.BetCount;
        //    var activeDays = this.ActiveDays;

        //    switch (this.CategoryID)
        //    {
        //        case (int)ClassificationSourceType.NewMember:
        //        case (int)ClassificationSourceType.NewMember31:
        //        case (int)ClassificationSourceType.NewMember32:
        //        case (int)ClassificationSourceType.NewMember33:
        //        case (int)ClassificationSourceType.NormalAccount:
        //        case (int)ClassificationSourceType.NormalAccount121:
        //        case (int)ClassificationSourceType.NormalAccount122:
        //        case (int)ClassificationSourceType.NormalAccount123:
        //        case (int)ClassificationSourceType.Probation:
        //        case (int)ClassificationSourceType.Probation51:
        //        case (int)ClassificationSourceType.Probation52:
        //        case (int)ClassificationSourceType.Probation53:
        //        case (int)ClassificationSourceType.SmartPunter:
        //        case (int)ClassificationSourceType.SmartPunter61:
        //        case (int)ClassificationSourceType.SmartPunter62:
        //        case (int)ClassificationSourceType.SmartPunter63:
        //        case (int)ClassificationSourceType.HighRiskPunter:
        //        case (int)ClassificationSourceType.HighRiskPunter71:
        //        case (int)ClassificationSourceType.HighRiskPunter72:
        //        case (int)ClassificationSourceType.HighRiskPunter73:
        //            spanReason += $"Win loss: {winlost} | Margin: {margin} | Bet Count: {betCount}";
        //            break;
        //        case (int)ClassificationSourceType.GoodMember:
        //        case (int)ClassificationSourceType.GoodMember111:
        //        case (int)ClassificationSourceType.GoodMember112:
        //        case (int)ClassificationSourceType.GoodMember113:
        //            spanReason += $"Turnover: {turnOver} | Win loss: {winlost} | Margin: {margin} | Bet Count: {betCount} | Active Day: {activeDays}";
        //            break;
        //        default: break;
        //    }

        //    return spanReason;
        //}
    }
}
