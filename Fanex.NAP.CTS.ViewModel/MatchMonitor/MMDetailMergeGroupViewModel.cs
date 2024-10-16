namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using System.Collections.Generic;
    using System.Linq;

    public class MMDetailMergeGroupViewModel
    {
        public MMDetailMergeGroupViewModel()
        {
            this.TicketMergeGroups = Enumerable.Empty<MMDetailMergeGroupModel>();
            this.Groups = Enumerable.Empty<GroupInfoViewModel>();
            this.TicketType = TicketType.Single;
        }

        public TicketType TicketType { get; set; }

        public IEnumerable<MMDetailMergeGroupModel> TicketMergeGroups { get; set; }

        public IEnumerable<GroupInfoViewModel> Groups { get; set; }

        public IEnumerable<GroupInfoViewModel> GroupByGroupID()
        {
            var groupArrangeList = new List<GroupInfoViewModel>();

            if (this.TicketMergeGroups.Any())
            {
               
                var groupIdx = 1;
                var groupList = this.TicketMergeGroups.Select(x => x.MergeGroupID).Distinct().OrderBy(x => x);

                foreach (var groupID in groupList)
                {
                    if (groupID == 0)
                    {
                        groupArrangeList.Add(new GroupInfoViewModel { GroupID = 0, GroupName = "-" });
                    }
                    else 
                    {
                        if (groupIdx != groupID)
                            groupIdx++;
                        groupArrangeList.Add(new GroupInfoViewModel { GroupID = groupID, GroupName = groupIdx.ToString() });
                    }                                       
                }
            }

            return groupArrangeList;
        }
    }
}
