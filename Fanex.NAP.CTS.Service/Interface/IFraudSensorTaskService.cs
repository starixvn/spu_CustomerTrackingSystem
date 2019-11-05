namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.DTO;
    using System.Collections.Generic;

    public interface IFraudSensorTaskService
    {
        IEnumerable<Match> BigStakeNearGoalExecutePrecondition();

        void BigStakeNearGoalExecuteRule(string matchIds);

        IEnumerable<Match> FlaggedAccountExecutePrecondition();

        void FlaggedAccountExecuteRule(string matchIds, string flaggedAccounts);

        IEnumerable<Match> DelayGoalExecutePrecondition();

        void DelayGoalExecuteRule(string matchIds);
    }
}