namespace Fanex.NAP.CTS.Model.API.FPS.SearchAccount
{
    using System;

    public class SearchAccountItemModel
    {
        public long CTSCustID { get; set; }
       
        public string Username { get; set; }

        public string RegisterName { get; set; } = string.Empty;

        public int SubscriberID { get; set; }

    }
}
