namespace Fanex.NAP.CTS.Library.Services
{
    using Data.Repository;
    using Fanex.NAP.CTS.Library.Core.BaseClasses;
    using Models;

    public interface IFlaggedAccountService
    {
        DBResult InsertAccounts(CommandBase criteria);

        DBResult DeleteAccounts(CommandBase criteria);
    }
}