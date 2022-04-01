namespace Fanex.NAP.CTS.Model.Account
{
    using System.Collections.Generic;

    public class AssociatedAccountPagingModel
    {
        public AssociatedAccountPagingModel()
        {
            AssociationList = new List<AssociatedAccountModel>();
        }

        public IEnumerable<AssociatedAccountModel> AssociationList { get; set; }

        public int TotalItems { get; set; }
    }
}
