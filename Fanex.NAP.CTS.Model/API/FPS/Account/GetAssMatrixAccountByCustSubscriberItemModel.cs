namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    using System;

    public class GetAssMatrixAccountByCustSubscriberItemModel
    {
        public int ID { get; set; }

        public long CTSCustID { get; set; }

        public string UserName { get; set; }

        public string RegisterName { get; set; } = string.Empty;

        public int DeviceID { get; set; }

        public string FirstDeviceCode { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
