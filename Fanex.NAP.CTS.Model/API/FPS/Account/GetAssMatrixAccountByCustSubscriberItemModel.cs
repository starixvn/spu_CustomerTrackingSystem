namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    using System;

    public class GetAssMatrixAccountByCustSubscriberItemModel
    {
        public int DeviceID { get; set; }

        public string DeviceNo { get; set; }

        public int AccountID { get; set; }

        public string UserName { get; set; }

        public DateTime LastAccessedDate { get; set; }

        public DateTime FirstSeenDate { get; set; }

        public int EXCEPTIONID { get; set; }

        public int Level { get; set; }

        public long CTSCustID { get; set; }

        public string UserName2 { get; set; }
    }
}
