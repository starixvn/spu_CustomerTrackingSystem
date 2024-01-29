namespace Fanex.NAP.CTS.Model.API
{
    public class DetectAssociationByTypeFilterModel
    {
        public DetectAssociationByTypeFilterModel(long CustID, AssociationDetectionType associationType)
        {
            this.CustID = CustID;
            this.IsDevice = false;
            this.IsAI = false;
            this.IsIP = false;

            switch (associationType)
            {
                case AssociationDetectionType.Device:
                    this.IsDevice = true;
                    break;
                case AssociationDetectionType.AI:
                    this.IsAI = true;
                    break;
                case AssociationDetectionType.IP:
                    this.IsIP = true;
                    break;
            }
        }

        public long CustID { get; set; }

        public bool IsDevice { get; set; }

        public bool IsAI { get; set; }

        public bool IsIP { get; set; }
    }
}
