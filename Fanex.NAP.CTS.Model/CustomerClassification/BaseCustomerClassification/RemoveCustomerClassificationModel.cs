namespace Fanex.NAP.CTS.Model.CustomerClassification
{   
    public class RemoveCustomerClassificationModel
    {
        public string CustClassificationJson { get; set; }

        public int FromAction { get; set; }

        public int UplineRoleID { get; set; }

        public long? QueueID { get; set; }

        public bool? IsAffectDownline { get; set; }

        public bool IsFromQueue { get; set; }
    }
}
