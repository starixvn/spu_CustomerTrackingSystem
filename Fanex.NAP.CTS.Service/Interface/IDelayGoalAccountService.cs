namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using System.Collections.Generic;

    public interface IDelayGoalAccountService
    {
        DBResult InsertAccounts(CommandBase criteria);

        IEnumerable<int> GetManualAccounts();
    }
}