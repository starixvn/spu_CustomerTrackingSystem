namespace Fanex.NAP.CTS.Library.Models.MatchMonitor.FraudCaseDetails
{
    using Fanex.NAP.CTS.Library.Core.BaseClasses;
    public class BetTypes
    {
        public int BetType { get; set; }

        public string BetTypeName
        {
            get
            {
                return CachedRef.GetBettypeName(BetType);
            }
        }
    }
}
