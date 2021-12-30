namespace Fanex.NAP.CTS.Model.Account.CustomerInfo
{
    using System;
  
    public class CustomerInfoDateModel
    {
        public DateTime NextCategoryScan { get; set; }

        public DateTime LastTicket { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime Created { get; set; }
    }
}
