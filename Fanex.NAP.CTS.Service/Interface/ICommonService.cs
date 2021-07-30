namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.ViewModel.Common;
    using System.Collections.Generic;

    public interface ICommonService
    {
        IEnumerable<SubscriberGroupViewModel> GetSubscriberGroups();

        IEnumerable<SubscriberGroupModel> GetSubscribers();

        IEnumerable<ValidateUserNameModel> ValidateUsernames(string userNames, out bool isException);
    }
}
