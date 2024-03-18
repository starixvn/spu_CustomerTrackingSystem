namespace Fanex.NAP.CTS.Model.SpecialCustomerClass
{
    using System.Collections.Generic;

    public class AddSpecialCCParamsModel
    {
        public List<UserListSpecialCCParamsModel> UserList { get; set; }

        public int CustomerClass { get; set; }

        public string Comment { get; set; }
    }
}
