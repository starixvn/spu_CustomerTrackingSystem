namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;

    public interface IGraphService
    {
        GraphCustomerItemViewModel GetGraphCustomerItemByShortestPathOfAccount(ShortestPathOfAccount shortestPathOfAccount);
    }
}