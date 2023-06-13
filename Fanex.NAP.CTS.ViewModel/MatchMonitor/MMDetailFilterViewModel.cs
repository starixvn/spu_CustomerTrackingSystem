namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using System.Collections.Generic;
    using System.Linq;

    public class MMDetailFilterViewModel
    {
        public MMDetailFilterViewModel()
        {
            this.CurrencyList = new List<string>();
            this.HDPList = new List<decimal>();
            this.ScoreList = new List<string>();
            this.ReasonList = new List<string>();
            this.GroupList = new List<GroupInfoViewModel>();
            this.ReasonInfos = string.Empty;
            this.ReasonGroupIDDic = new Dictionary<string, List<int>>();
        }

        public MMDetailFilterViewModel(MMTicketDetailInfoModel mMTicket, MatchMonitorDetailListViewModel fraudTicket)
        {
            RecordsTotal = mMTicket.TotalTicket;
            MaxSequenceId = mMTicket.MaxSequenceId;
            CurrencyList = mMTicket.ListCurrency?.Split(',').ToList() ?? new List<string>() { "-" };
            HDPList = mMTicket.ListHDP?.Split(',').Select(x => decimal.Parse(x)).ToList() ?? new List<decimal>() { };
            ScoreList = mMTicket.ListScore?.Split(',').ToList() ?? new List<string>() { "-" };
            
            var reasonList = fraudTicket.ReasonList?.ToList() ?? new List<string>();
            var groupList = fraudTicket.GroupList?.ToList() ?? new List<GroupInfoViewModel>();
            var reasonGroupIDDic = fraudTicket?.ReasonGroupIDDic ?? new Dictionary<string, List<int>>();

            if (fraudTicket.HomeMatchItemList.Count() + fraudTicket.AwayMatchItemList.Count() < mMTicket.TotalTicket)
            {
                if (!reasonList.Any(x => x == "-")) reasonList.Insert(0, "-");
                if (!reasonGroupIDDic.Any(x => x.Key == "-")) reasonGroupIDDic.Add("-", new List<int>() { 0 });
                if (!groupList.Any(x => x.GroupName == "-")) groupList.Insert(0, new GroupInfoViewModel
                {
                    GroupID = 0,
                    GroupName = "-"
                });
            }

            ReasonList = reasonList;
            GroupList = groupList;
            ReasonInfos = string.Join(",", ReasonList.Where(x => !x.Contains("-")));
            ReasonGroupIDDic = reasonGroupIDDic.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        public bool IsFilter { get; set; }

        public long? MaxSequenceId { get; set; }

        public long RecordsTotal { get; set; }

        public List<string> CurrencyList { get; set; }

        public List<decimal> HDPList { get; set; }

        public List<string> ScoreList { get; set; }

        public List<string> ReasonList { get; set; }

        public List<GroupInfoViewModel> GroupList { get; set; }

        public string ReasonInfos { get; set; }

        public Dictionary<string, List<int>> ReasonGroupIDDic { get; set; }

        public List<string> AGroupList { get; set; }
    }
}
