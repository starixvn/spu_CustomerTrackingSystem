namespace Fanex.NAP.CTS.ViewModel.Account
{
    using Fanex.NAP.CTS.Model.Account;

    public class AccountProfileViewModel
    {
        public CustomerGeneralStatusModel GeneralStatusInfo { get; set; }

        public string StatusLabel { get; set; }

        public string FraudCases { get; set; }

        public CustomerRobotUserStatusModel RobotUserStatus { get; set; }

        public CustomerFlagInfoModel FlagInfo { get; set; }

        public CustomerEsportAutoSuspendStatusModel EsportAutoSuspendStatus { get; set; }
    }
}
