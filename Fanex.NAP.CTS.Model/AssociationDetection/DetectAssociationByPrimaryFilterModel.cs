namespace Fanex.NAP.CTS.Model.AssociationDetection
{
    public class DetectAssociationByPrimaryFilterModel
    {
        public DetectAssociationByPrimaryFilterModel(string CTSCustIDs, AssociationDetectionType associationType)
        {
            this.CTSCustIDs = CTSCustIDs;
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

        public string CTSCustIDs { get; set; }

        public bool IsDevice { get; set; }

        public bool IsAI { get; set; }

        public bool IsIP { get; set; }
    }
}
