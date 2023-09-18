namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{   
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MMDetailTicketListViewModel
    {
        public MMDetailTicketListViewModel() : base()
        {
            this.CTSCustIDList = string.Empty;
            this.HomeMatchItemList = new List<MatchMonitorDetailItemViewModel>();
            this.AwayMatchItemList = new List<MatchMonitorDetailItemViewModel>();
            this.ItemList = new List<MatchMonitorDetailItemViewModel>();            
        }

        private MMDetailTicketListViewModel(MMDetailTicketListViewModel matchMonitorDetailList) : base()
        {
            this.LastSequenceID = matchMonitorDetailList.LastSequenceID;
            this.CTSCustIDList = matchMonitorDetailList.CTSCustIDList;
            this.HomeMatchItemList = matchMonitorDetailList.HomeMatchItemList.ToList();
            this.AwayMatchItemList = matchMonitorDetailList.AwayMatchItemList.ToList();
            this.ItemList = matchMonitorDetailList.ItemList.ToList();           
        }                
           
        public long RecordsTotal { get; set; }

        public bool IsLive { get; set; }

        public int BetTypeID { get; set; }

        public long? LastSequenceID { get; set; }

        public string CTSCustIDList { get; set; }

        public bool IsSingleGrid { get; set; }

        public List<MatchMonitorDetailItemViewModel> HomeMatchItemList { get; set; }

        public List<MatchMonitorDetailItemViewModel> AwayMatchItemList { get; set; }

        public List<MatchMonitorDetailItemViewModel> ItemList { get; set; }

        public MMDetailTicketListViewModel Clone() => new MMDetailTicketListViewModel(this);
               
        public void BindReasons(IEnumerable<string> rawReasons, MMDetailFilterViewModel filter)
        {
            var reasons = new List<string>();

            foreach (var reason in rawReasons)
            {
                if (!string.IsNullOrEmpty(reason))
                    reasons.AddRange(reason.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }

            filter.ReasonList = reasons.Distinct().ToList();
            filter.ReasonInfos = string.Join(", ", filter.ReasonList);
            filter.ReasonGroupIDDic = new Dictionary<string, List<int>>();

            var reasonGroupIDList = new List<int>();

            foreach (var reason in filter.ReasonList)
            {
                var reasonGroupIDHs = this.HomeMatchItemList.Where(x => x.ReasonList.Contains(reason)).Select(x => x.ADetectGroupID);
                var reasonGroupIDAs = this.AwayMatchItemList.Where(x => x.ReasonList.Contains(reason)).Select(x => x.ADetectGroupID);

                reasonGroupIDList.Clear();
                reasonGroupIDList.AddRange(reasonGroupIDHs);
                reasonGroupIDList.AddRange(reasonGroupIDAs);

                filter.ReasonGroupIDDic.Add(reason, reasonGroupIDList.Distinct().ToList());
            }

        }     
       
    }
}