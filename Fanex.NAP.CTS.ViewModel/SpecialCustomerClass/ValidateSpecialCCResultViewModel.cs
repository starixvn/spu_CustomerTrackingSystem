namespace Fanex.NAP.CTS.ViewModel.SpecialCustomerClass
{
    using System.Collections.Generic;

    public class ValidateSpecialCCResultViewModel
    {
        public IEnumerable<ValidateSpecialCCItemViewModel> Users { get; set; }
    }

    public class ValidateSpecialCCItemViewModel
    {
        public string Username { get; set; }

        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        public int SportID { get; set; }

        public int ValidateID { get; set; }

        public bool IsValid { get; set; }
    }
}
