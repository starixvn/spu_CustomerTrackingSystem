namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.DTO;
    using System.Collections.Generic;

    public interface INotificationService
    {
        IEnumerable<Message> Get(int custId, int maxAssociationID);
    }
}