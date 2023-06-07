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
            this.ScoreList = Enumerable.Empty<string>();
            this.ReasonList = Enumerable.Empty<string>();
            this.GroupList = Enumerable.Empty<GroupInfoViewModel>();
            this.ReasonInfos = string.Empty;
            this.ReasonGroupIDDic = new Dictionary<string, List<int>>();
        }

        public MMDetailFilterViewModel(MMTicketDetailInfoModel mMTicket, MatchMonitorDetailListViewModel fraudTicket)
        {
            RecordsTotal = mMTicket.TotalTicket;
            CurrencyList = mMTicket.ListCurrency?.Split(',') ?? new List<string>() { "-" }.AsEnumerable();
            HDPList = mMTicket.ListHDP?.Split(',').Select(x => decimal.Parse(x)) ?? new List<decimal>() { };
            ScoreList = mMTicket.ListScore?.Split(',') ?? new List<string>() { "-" }.AsEnumerable();
            var reasonList = fraudTicket.ReasonList?.ToList() ?? new List<string>();
            var groupList = fraudTicket.GroupList?.ToList() ?? new List<GroupInfoViewModel>();
            var reasonGroupIDDic = fraudTicket?.ReasonGroupIDDic ?? new Dictionary<string, List<int>>();

            if (fraudTicket.HomeMatchItemList.Count() + fraudTicket.AwayMatchItemList.Count() < mMTicket.TotalTicket)
            {
                if (!reasonList.Any(x => x == "-")) reasonList.Add("-");
                if (!reasonGroupIDDic.Any(x => x.Key == "-")) reasonGroupIDDic.Add("-", new List<int>() { 0 });
                if (!groupList.Any(x => x.GroupName == "-")) groupList.Add(new GroupInfoViewModel
                {
                    GroupID = 0,
                    GroupName = "-"
                });
            }

            ReasonList = reasonList;
            GroupList = groupList;
            ReasonInfos = string.Join(",", ReasonList.Where(x => !x.Contains("-")));
            ReasonGroupIDDic = reasonGroupIDDic;
        }

        public bool isFilter { get; set; }

        public long RecordsTotal { get; set; }

        public IEnumerable<string> CurrencyList { get; set; }

        public IEnumerable<decimal> HDPList { get; set; }

        public IEnumerable<string> ScoreList { get; set; }

        public IEnumerable<string> ReasonList { get; set; }

        public IEnumerable<GroupInfoViewModel> GroupList { get; set; }

        public string ReasonInfos { get; set; }

        public Dictionary<string, List<int>> ReasonGroupIDDic { get; set; }

        public string CurrencyListString { get; set; }

        public string HDPListString { get; set; }

        public string ScoreListString { get; set; }

        public string ReasonListString { get; set; }

        public string ReasonGroupIDDicString { get; set; }
    }
}
