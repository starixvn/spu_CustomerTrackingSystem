namespace Fanex.NAP.CTS.Model.API.FPS.Account
{
    using System;

    public class GetAssMatrixAccountByCustSubscriberItemDCSExtraModel
    {
        public int ID { get; set; }

        public long AccountID { get; set; }

        public string LoginName { get; set; }

        public int DeviceID { get; set; }

        public string FirstDeviceCode { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
