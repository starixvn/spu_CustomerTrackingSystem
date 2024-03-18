namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.SpecialCustomerClass;
    using Fanex.NAP.CTS.ViewModel.SpecialCustomerClass;
    using System.Collections.Generic;

    public interface ISpecialCustomerClassService
    {
        IEnumerable<CustomerClassModel> GetAllCustomerClass(string username);

        ResultInsertCustomerClass InsertCustomerClass(string username, int custClass, bool isAffectToAssociation, int createdBy);

        DBResult RemoveCustomerClass(long ctsCustID, int createdBy);

        ValidateSpecialCCResultViewModel ValidateSpecialCC(FunctionValidateSpecialCC functionID, List<UserListSpecialCCParamsModel> userList);
    }
}
