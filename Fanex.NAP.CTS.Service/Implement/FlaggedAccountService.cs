namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.BaseClasses;

    public class FlaggedAccountService : IFlaggedAccountService
    {
        public DBResult InsertAccounts(CommandBase criteria)
        {
            return DataService.UpdateData(criteria, "Insert Flagged Account");
        }

        public DBResult DeleteAccounts(CommandBase criteria)
        {
            return DataService.UpdateData(criteria, "Delete Flagged Account");
        }
    }
}