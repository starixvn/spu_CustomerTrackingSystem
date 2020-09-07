namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.SpecialCustomerClass;
    using System.Collections.Generic;

    public interface ISpecialCustomerClassService
    {
        IEnumerable<SpecialCustomerClassModel> GetAllCustomerClass();

        InsertSpecialCustomerClassResult InsertCustomerClass(string username, int custClass, bool isAffectToAssociation, int createdBy);

        int SyncCustomerClassToMainDb(IEnumerable<SpecialCustomerClassForMainDb> customerClassList);

        int RemoveCustomerClass(long ctsCustId);
    }
}
