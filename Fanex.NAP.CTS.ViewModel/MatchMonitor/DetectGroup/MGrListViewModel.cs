namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.DetectGroup
{
    using System.Collections.Generic;
    using System.Linq;

    public class MGrListViewModel
    {
        public MGrListViewModel()
        {
            this.MGrItems = new List<MGrItemViewModel>();
        }

        public IEnumerable<MGrItemViewModel> MGrItems { get; set; }

        public int TotalTicketDetectGroup { get; set; }

        public int TotalTicketHasGroup { get; set; }

        public void BuildMGrList(IEnumerable<DetectGroupAssociationItemViewModel> mGrDetectItems
            , IEnumerable<TicketSelectForMGrViewModel> mGrOriItems)
        {
            var mGrItems = new List<MGrItemViewModel>();
            var countCustDetecteds = mGrDetectItems.Select(x => x.CustID).Distinct().Count();
            var countCusts = mGrOriItems.Select(x => x.CustID).Distinct().Count();

            if (countCusts > countCustDetecteds)
            {
                mGrItems.Add(new MGrItemViewModel
                {
                    MDetectGroupID = 0,
                    MDetectGroup = "-"
                });
            }

            mGrItems.AddRange(mGrDetectItems.GroupBy(x => new { x.GroupID, x.GroupName })
               .Select(g => new MGrItemViewModel
               {
                   MDetectGroupID = g.Key.GroupID.HasValue ? g.Key.GroupID.Value : 0,
                   MDetectGroup = g.Key.GroupName
               }).OrderBy(x => x.MDetectGroupID));

            this.MGrItems = mGrItems;
        }
    }
}
