namespace Fanex.NAP.CTS.Model.GraphDB
{
    using System.Collections.Generic;

    public class DirectAssociationDeviceModel
    {
        public List<int> DeviceIDs { get; set; }

        public int TotalDirectAssociation { get; set; }

        public IEnumerable<DirectAssociationModel> DirectAssociation { get; set; }
    }
}
