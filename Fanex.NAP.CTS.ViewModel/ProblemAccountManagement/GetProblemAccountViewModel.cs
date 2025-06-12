namespace Fanex.NAP.CTS.ViewModel.ProblemAccountManagement
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using Fanex.NAP.CTS.Resource.Consts;
   
    public class GetProblemAccountViewModel
    {
        public GetProblemAccountViewModel(GetProblemAccountModel data)
        {
            this.UserName = data.UserName;
            this.Site = data.Site;
            this.Currency = data.Currency;
            this.Category = data.CategoryName;            
            this.CreatedBy = data.CreatedBy;
            this.CreatedDateStr = data.CreatedDate.ToString(Consts.FormatDateToString_MMMddyyyy);
            this.TVSRequestID = data.TVSRequestID;                 
            this.IssueTypeID = data.IssueTypeID;
            this.SportName = data.SportType.ToString();
            //this.SportName = data.SportType != null ? CachedRef.GetSportTypeName(data.SportType.Value) : string.Empty; comment for check KL RemotingRef Not Working

            this.Comment = this.BuildComment(data);
        }

        public string UserName { get; set; }

        public string Site { get; set; }

        public string Currency { get; set; }

        public string Category { get; set; }

        public string Comment { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedDateStr { get; set; }

        public int? TVSRequestID { get; set; }      
       
        public string SportName { get; set; }

        public int? IssueTypeID { get; set; }

        private string BuildComment(GetProblemAccountModel data)
        {
            var comment = string.Empty;

            if (string.IsNullOrEmpty(data.Remark) && data.IsFromTVS)
            {
                if (data.TVSRequestID > 0)
                    comment += $"Void by RequestID: { data.TVSRequestID } ";
                else
                    comment += $"Void by BetID ";

                if (data.SportType != null && data.SportType > 0)
                    comment += $"| Sport: {this.SportName} ";

                if (data.IsParlay != null && data.IsParlay.Value)
                    comment += $"| Parlay";
            }
            else
            {
                comment = string.IsNullOrEmpty(data.Remark) ? "-" : data.Remark;
            }

            return comment;
        }

    }
}
