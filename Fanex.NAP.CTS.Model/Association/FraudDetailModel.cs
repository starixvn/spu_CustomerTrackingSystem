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
    }
}