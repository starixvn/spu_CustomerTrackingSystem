namespace Fanex.NAP.CTS.ViewModel.Account
{
    using Fanex.NAP.CTS.Model.Account;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AccountProfileViewModel
    {
        public CustomerGeneralStatusModel GeneralStatusInfo { get; set; }

        public string StatusLabel { get; set; }

        public SuspiciousCustomerModel SuspiciousInfo { get; set; }

        public string FraudCases { get; set; }

        public CustomerRobotUserStatusModel RobotUserStatus { get; set; }

        public CustomerFlagInfoModel FlagInfo { get; set; }

        public CustomerEsportAutoSuspendStatusModel EsportAutoSuspendStatus { get; set; }
    }
}
