namespace Fanex.NAP.CTS.Model.Association
{
    using Fanex.NAP.CTS.Model.Transaction;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public abstract class FraudDetailModel
    {
        public long MatchID { get; set; }

        public DateTime EventDate { get; set; }

        public string FraudTransList { get; set; }

        private List<string> _fraudTransListSperation = null;
        public IEnumerable<string> FraudTransListSperation
        {
            get
            {
                if (_fraudTransListSperation != null)
                {
                    return _fraudTransListSperation;
                }
                if (string.IsNullOrEmpty(FraudTransList))
                {
                    _fraudTransListSperation = new List<string>();
                }
                else
                {
                    char[] sperator = new char[] { ',' };
                    _fraudTransListSperation = FraudTransList.Split(sperator, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                return _fraudTransListSperation;
            }
        }

        public bool IsCustId1 { get; set; }

        public void DefineCustIdTypeInCoupleIPFraudDetails(IEnumerable<TicketDetailsModel> ticketDetailsModels, long custId1)
        {
            var ticketDetailsByCustId1 = ticketDetailsModels.Where(p => p.CustID == custId1).OrderByDescending(desc => desc.StakeRM).ToList();
            var tranIds = ticketDetailsByCustId1.Select(s => s.TransId).ToArray();

            var tranId = FraudTransListSperation.FirstOrDefault();
            if (!string.IsNullOrEmpty(tranId) && tranIds.Any(p => p.ToString() == tranId))
            {
                IsCustId1 = true;
            }
            else
            {
                IsCustId1 = false;
            }
        }
    }
}