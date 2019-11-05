namespace Fanex.NAP.CTS.Service
{
    using Core.BaseClasses;
    using Criteria.FraudSensorTask;
    using Fanex.NAP.CTS.Model.DTO;
    using System.Collections.Generic;

    public class FraudSensorTaskService : IFraudSensorTaskService
    {
        public IEnumerable<Match> BigStakeNearGoalExecutePrecondition()
        {
            return DataService.GetData<Match>(new BigStakeNearGoalPreconditionCriteria());
        }

        public void BigStakeNearGoalExecuteRule(string matchIds)
        {
            DataService.Execute(new BigStakeNearGoalRuleCriteria
            {
                MatchIds = matchIds
            });
        }

        public IEnumerable<Match> FlaggedAccountExecutePrecondition()
        {
            return DataService.GetData<Match>(new FlaggedAccountPreconditionCriteria());
        }

        public void FlaggedAccountExecuteRule(string matchIds, string flaggedAccounts)
        {
            DataService.Execute(new FlaggedAccountRuleCriteria
            {
                MatchIds = matchIds,
                FlaggedAccountsXML = flaggedAccounts
            });
        }

        public IEnumerable<Match> DelayGoalExecutePrecondition()
        {
            return DataService.GetData<Match>(new DelayGoalPreconditionCriteria());
        }

        public void DelayGoalExecuteRule(string matchIds)
        {
            DataService.Execute(new DelayGoalRuleCriteria
            {
                MatchIds = matchIds
            });
        }
    }
}