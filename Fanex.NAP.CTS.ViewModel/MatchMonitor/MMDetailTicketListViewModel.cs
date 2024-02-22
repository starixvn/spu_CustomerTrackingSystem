namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
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

        public string LastTransDate { get; set; }

        public string CTSCustIDList { get; set; }

        public bool IsSingleGrid { get; set; }

        public List<MatchMonitorDetailItemViewModel> HomeMatchItemList { get; set; }

        public List<MatchMonitorDetailItemViewModel> AwayMatchItemList { get; set; }

        public List<MatchMonitorDetailItemViewModel> ItemList { get; set; }

        public MMDetailTicketListViewModel Clone() => new MMDetailTicketListViewModel(this);

        public void SetLastMark(MMDetailInfoViewModel info)
        {
            LastSequenceID = info.LastScannedSequenceID;
            LastTransDate = info.LastTransDate;
        }
    }
}