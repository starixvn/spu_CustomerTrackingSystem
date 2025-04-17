namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn;
    using System;
    using System.Collections.Generic;
    using System.Linq;   

    public class MMDetailFilterViewModel
    {
        public MMDetailFilterViewModel()
        {
            this.ReasonInfos = string.Empty;
            this.RecordsTotal = 0;
            this.SubscriberTypeList = new List<string>();
            this.CurrencyList = new List<KeyValuePair<int, string>>();
            this.HDPList = new List<decimal>();
            this.BetTeamList = new List<string>();
            this.ScoreList = new List<string>();
            this.ReasonList = new List<string>();
            this.GroupList = new List<GroupInfoViewModel>();
            this.MGroupList = new List<string>();
            this.MGrTranIDListFiltered = "";
            this.DropdownListFilter = new MMDetailFilterDropdownListViewModel();
            this.DropdownListFilterDependOn = new List<DependOnFilterItemViewModel>();          
        }
      
        protected MMDetailFilterViewModel(MMDetailFilterViewModel filter)
        {
            this.IsFilter = filter.IsFilter;
            this.IsFilterSuspicious = filter.IsFilterSuspicious;
            this.MaxSequenceId = filter.MaxSequenceId;
            this.MaxTransDate = filter.MaxTransDate;
            this.RecordsTotal = filter.RecordsTotal;
            this.ScoreList = filter.ScoreList;
            this.Danger1List = filter.Danger1List;
            this.Danger2List = filter.Danger2List;
            this.Danger3List = filter.Danger3List;
            this.Danger4List = filter.Danger4List;
            this.Danger5List = filter.Danger5List;
            this.CurrencyList = filter.CurrencyList;
            this.CurrencyIdList = filter.CurrencyIdList;
            this.ReasonInfos = filter.ReasonInfos;
            this.ReasonGroupIDDic = filter.ReasonGroupIDDic;
            this.AGroupList = filter.AGroupList;
            this.MGroupList = filter.MGroupList;
            this.ReasonList = filter.ReasonList;
            this.GroupList = filter.GroupList;
            this.HDPList = filter.HDPList;
            this.BetTeamList = filter.BetTeamList;
            this.CustAmountRM = filter.CustAmountRM;
            this.TicketStatusList = filter.TicketStatusList;
            this.SubscriberTypeList = filter.SubscriberTypeList;
            this.IsDangerNone = filter.IsDangerNone;
            this.MGrTranIDListFiltered = filter.MGrTranIDListFiltered;
            this.DropdownListFilterDependOn = filter.DropdownListFilterDependOn;
        }

        public bool IsFilter { get; set; }

        public bool IsFilterSuspicious { get; set; }

        public bool IsDisplaySingleGrid { get; set; }

        public int SportType { get; set; }

        public long? MaxSequenceId { get; set; }

        public string MaxTransDate { get; set; }

        public long RecordsTotal { get; set; }

        public List<string> SubscriberTypeList { get; set; }

        public List<int> CustomerClassList { get; set; }

        public List<KeyValuePair<int, string>> CurrencyList { get; set; }

        public List<int> CurrencyIdList { get; set; }

        public List<decimal> HDPList { get; set; }

        public List<string> BetTeamList { get; set; }

        public List<string> ScoreList { get; set; }

        public bool IsDangerNone { get; set; }

        public List<int> Danger1List { get; set; }

        public List<int> Danger2List { get; set; }

        public List<int> Danger3List { get; set; }

        public List<int> Danger4List { get; set; }

        public List<int> Danger5List { get; set; }      

        public List<string> ReasonList { get; set; }
        
        public decimal CustAmountRM { get; set; }
        
        public List<GroupInfoViewModel> GroupList { get; set; }

        public string ReasonInfos { get; set; }

        public Dictionary<string, List<int>> ReasonGroupIDDic { get; set; }

        public List<string> AGroupList { get; set; }

        public List<string> MGroupList { get; set; }

        public List<int> TicketStatusList { get; set; }

        public string MGrTranIDListFiltered { get; set; }

        public MMDetailFilterDropdownListViewModel DropdownListFilter { get; set; }

        public IEnumerable<DependOnFilterItemViewModel> DropdownListFilterDependOn { get; set; }

        public MMDetailFilterViewModel Clone() => new MMDetailFilterViewModel(this);              
       
        public List<string> OrderByFormatScoreBetTeam(IEnumerable<string> betTeamList, string separator)
        {
            var resultList = new List<string>();
            var separatorDisplay = string.Empty;
            var betTeamOrderList = new List<BetTeamOrderViewModel>();
            var betTeamOrderOtherList = new List<BetTeamOrderViewModel>();           

            foreach (var betTeam in betTeamList)
            {
                var arr = betTeam.Split('-');

                if (arr.Length == 1)
                {
                    arr = betTeam.Split(':');
                    separatorDisplay = ":";
                }
                else 
                {
                    separatorDisplay = separator;
                }

                if (arr.Length > 1)
                {
                    int.TryParse(arr[0], out int choiceA);

                    betTeamOrderList.Add(new BetTeamOrderViewModel
                    {
                        ChoiceA = choiceA,
                        ChoiceB = arr[1].Trim(),
                        ChoiceDisplay = choiceA + separatorDisplay + arr[1].Trim()
                    });
                }
                else
                {
                    betTeamOrderOtherList.Add(new BetTeamOrderViewModel
                    {
                        ChoiceDisplay = betTeam
                    });
                }
            }

            resultList.AddRange(betTeamOrderList.OrderBy(x => x.ChoiceA).ThenBy(x => x.ChoiceB).Select(x => x.ChoiceDisplay));
            resultList.AddRange(betTeamOrderOtherList.OrderBy(x => x.ChoiceDisplay).Select(x => x.ChoiceDisplay));

            return resultList;
        }

    }
}
