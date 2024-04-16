namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.SpecialCustomerClass;
    using Fanex.NAP.CTS.ViewModel.SpecialCustomerClass;
    using System.Collections.Generic;

    public interface ISpecialCustomerClassService
    {
        IEnumerable<CustomerClassViewModel> GetSpecialCustomerClass(SearchSpecialCCParamsModel model);

        void InsertCustomerClass(AddSpecialCCParamsModel model, ValidateSpecialCCResultViewModel validateSpecialCC, int createdBy);

        void RemoveCustomerClass(RemoveSpecialCCParamsModel model, ValidateSpecialCCResultViewModel validateUsers, int removedBy);

        ValidateSpecialCCResultViewModel ValidateSpecialCC(FunctionValidateSpecialCC functionID, List<UserListSpecialCCParamsModel> userList);
    }
}
