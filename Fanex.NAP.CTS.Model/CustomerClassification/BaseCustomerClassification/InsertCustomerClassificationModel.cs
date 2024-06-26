namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    public class InsertCustomerClassificationModel
    {
        public string CustClassificationJson { get; set; }

        public bool IsToQueue { get; set; }

        public int FromAction { get; set; }

        public int UplineRoleID { get; set; }

        public long QueueID { get; set; }

        public bool AffectDownline { get; set; }
    }
}
