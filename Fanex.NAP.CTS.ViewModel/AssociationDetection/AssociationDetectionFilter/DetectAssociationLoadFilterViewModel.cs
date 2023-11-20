namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{
    using Fanex.NAP.CTS.Model.Common;
    using System.Collections.Generic;
    using System.Linq;

    public class DetectAssociationLoadFilterViewModel
    {
        public DetectAssociationLoadFilterViewModel(List<CCGroupModel> ccGroup, List<int> custClasses)
        {
            CustClassFilter = RenderCCFilter(ccGroup, custClasses);
            DangerFilter = new DetectAssociationDangerFilterViewModel();
        }

        public IEnumerable<CCGroupModel> CustClassFilter { get; set; }


        public DetectAssociationDangerFilterViewModel DangerFilter { get; set; }

        private IEnumerable<CCGroupModel> RenderCCFilter(List<CCGroupModel> ccGroup, List<int> custClasses)
        {
            var ccFilter = new List<CCGroupModel>();
            
            if (custClasses == null || !custClasses.Any())
            {
                return ccFilter;
            }

            if (ccGroup != null && ccGroup.Any())
            {
                ccGroup.FirstOrDefault(c => c.CCGroupName.Equals("Others")).CCList.Add(-1);
                
                ccGroup.ForEach(group =>
                {
                    var cc = group.CCList.Intersect(custClasses);

                    if (cc.Any())
                    {
                        ccFilter.Add(new CCGroupModel
                        {
                            CCGroupName = group.CCGroupName,
                            CCList = cc.ToList()
                        });
                    }
                });
            }

            return ccFilter;
        }
    }
}
