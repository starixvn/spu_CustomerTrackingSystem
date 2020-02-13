using System;

namespace Fanex.NAP.CTS.Model.Account
{
    public class AccountExceptionsModel
    {
        public Int64 ExceptionCTSCustId { get; set; }

        public int CustId { get; set; }

        public int CTSCustId { get; set; }

        public string UserName { get; set; }

        public string LoginId { get; set; }

        public DateTime ExceptionDate { get; set; }

        public string Comment { get; set; }

        public string CreatedBy { get; set; }
    }
}
