namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    using Fanex.NAP.CTS.Core.Extentions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DetectMGrDependOnViewModel
    {
        public DetectMGrDependOnViewModel() 
        {
            this.AGrMGrList = new List<AGrMGrViewModel>();
            this.MGrHDPList = new List<MGrHDPViewModel>();
        }

        public IEnumerable<AGrMGrViewModel> AGrMGrList { get; set; }

        public IEnumerable<MGrHDPViewModel> MGrHDPList { get; set; }
         
        public void BuildDependOnMGr(IEnumerable<DetectGroupAssociationItemViewModel> mGrDetectItems,
            IEnumerable<TicketSelectForMGrViewModel> mGrItems)
        {
            var newMGr = mGrDetectItems.Join(mGrItems,
                g => g.CustID, gd => gd.CustID,
                (g, gd) => {
                    var item = gd;
                    item.MGr = g.GroupID.HasValue ? g.GroupID.Value : 0;
                    return item;
                });

            this.AGrMGrList = newMGr.GroupBy(x => x.AGr)
                .Select(g => new AGrMGrViewModel
                {
                    AGr = g.Key,
                    MGrItems = g.Select(x => new AGrMGrItemViewModel { MGr = x.MGr.ToString() }).Distinct(x=>x.MGr)
                });
            
        }
    }
}
