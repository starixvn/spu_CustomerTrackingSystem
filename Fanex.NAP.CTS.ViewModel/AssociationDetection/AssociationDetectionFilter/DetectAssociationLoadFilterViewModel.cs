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
            var ccFull = new List<int>();

            if (custClasses == null || !custClasses.Any())
            {
                return ccFilter;
            }

            if (ccGroup != null && ccGroup.Any())
            {
                ccGroup.ForEach(group =>
                {
                    var cc = group.CCList.Intersect(custClasses);
                    ccFull.AddRange(group.CCList);

                    if (cc.Any())
                    {
                        ccFilter.Add(new CCGroupModel
                        {
                            CCGroupName = group.CCGroupName,
                            CCList = cc.ToList()
                        });
                    }
                });

                var otherOptions = custClasses.Except(ccFull);

                if (otherOptions.Any())
                {
                    var temp = ccFilter.FirstOrDefault(c => c.CCGroupName.Equals("Others"));

                    if (temp != null && temp.CCList.Any())
                    {
                        temp.CCList.AddRange(otherOptions);
                        temp.CCList = temp.CCList.OrderBy(c => c).ToList();
                    }
                    else
                    {
                        ccFilter.Add(new CCGroupModel
                        {
                            CCGroupName = "Others",
                            CCList = otherOptions.OrderBy(c => c).ToList()
                        });
                    }                   
                }
            }

            return ccFilter;
        }
    }
}