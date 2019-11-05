namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.BaseClasses;

    public interface IFlaggedAccountService
    {
        DBResult InsertAccounts(CommandBase criteria);

        DBResult DeleteAccounts(CommandBase criteria);
    }
}