namespace Fanex.NAP.CTS.Model.ManageSubscriber
{
    using Fanex.NAP.CTS.Core;
    using System.Collections.Generic;

    public class UpdateSubsciberResultModel
    {
        public DBResult Result { get; set; }

        public List<ReUpdateSubscribeBySiteIDModel> Subscribers { get; set; }
    }
}
