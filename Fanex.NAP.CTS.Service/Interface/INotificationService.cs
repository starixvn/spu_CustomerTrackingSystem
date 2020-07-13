namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.DTO;
    using System.Collections.Generic;

    public interface INotificationService
    {
        //int GetUnreadMessageCounter(int custId);

        IEnumerable<Message> Get(int custId, bool isRead);
    }
}