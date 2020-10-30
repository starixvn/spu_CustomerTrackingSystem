namespace Fanex.NAP.CTS.Model.GraphDB
{
    using System.Collections.Generic;

    public class DirectAssociationModel
    {
        public long? CustId { get; set; }

        public string UserName {get; set; }

        public List<int> DeviceIds { get; set; }
    }
}
