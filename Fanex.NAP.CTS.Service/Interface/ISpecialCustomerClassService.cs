namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.SpecialCustomerClass;
    using System.Collections.Generic;

    public interface ISpecialCustomerClassService
    {
        IEnumerable<CustomerClassModel> GetAllCustomerClass();

        ResultInsertCustomerClass InsertCustomerClass(string username, int custClass, bool isAffectToAssociation, int createdBy);

        DBResult RemoveCustomerClass(long ctsCustID, int createdBy);
    }
}
