namespace Fanex.NAP.CTS.Model.MatchMonitor.FraudCaseDetails
{
    using Fanex.NAP.CTS.Core.BaseClasses;

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
