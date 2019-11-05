namespace Fanex.NAP.CTS.Service
{
    using Core.BaseClasses;
    using Criteria.FraudCase.DelayedGoal;
    using Fanex.NAP.CTS.Core;
    using System.Collections.Generic;

    public class DelayGoalAccountService : IDelayGoalAccountService
    {
        public DBResult InsertAccounts(CommandBase criteria)
        {
            return DataService.UpdateData(criteria, "Insert Delay Goal Account");
        }

        public IEnumerable<int> GetManualAccounts()
        {
            return DataService.GetData<int>(new DelayGoalManualAccountGetCriteria());
        }
    }
}