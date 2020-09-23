namespace Fanex.NAP.CTS.ViewModel.Notification
{ 
    using Fanex.NAP.CTS.Model.DTO;
    using System.Collections.Generic;

    public class NotificationMessagesViewModel
    {
        public List<AssociatedMessageModel> AssociatedMessages { get; set; }

        public List<ProbationMessageModel> ProbationMessages { get; set; }
    }
}
