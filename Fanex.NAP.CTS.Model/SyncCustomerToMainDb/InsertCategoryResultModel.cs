namespace Fanex.NAP.CTS.Model.SyncCustomerToMainDb
{
    using System.Collections.Generic;

    public class InsertCategoryResultModel
    {
        public string ErrorMessage { get; set; }

        public IEnumerable<long> CustIds { get; set; }
    }
}
