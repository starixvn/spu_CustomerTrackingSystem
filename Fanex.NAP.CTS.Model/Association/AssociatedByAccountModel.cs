using System;

namespace Fanex.NAP.CTS.Model.Association
{
    public class AssociatedByAccountModel
    {
        public int Tag { get; set; }

        public string Username { get; set; }

        public string LoginId { get; set; }

        public int Status { get; set; }

        public string Subscriber { get; set; }

        public string AssociationType { get; set; }

        public int SharedDevices { get; set; }

        public int SharedIps { get; set; }

        public DateTime AssociationDate { get; set; }
    }
}
