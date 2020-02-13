using System;

namespace Fanex.NAP.CTS.ViewModel.Account.ChainToEvidence
{
    public class AccountExceptionsViewModel
    {
        public Int64 ExceptionCTSCustId { get; set; }

        public int CustId { get; set; }

        public int CTSCustId { get; set; }

        public string UserName { get; set; }

        public string LoginId { get; set; }

        public DateTime ExceptionDate { get; set; }

        public string Comment { get; set; }

        public string Creator { get; set; }                
    }
}
