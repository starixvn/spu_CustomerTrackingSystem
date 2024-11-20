namespace Fanex.NAP.CTS.Model.CustomerClassificationAgency.BaseCustomerClassificationAgency
{
    public class RemoveCustomerClassificationAgencyModel
    {
        public string CustClassificationJson { get; set; }

        public int FromAction { get; set; }

        public int UplineRoleID { get; set; }

        public long? QueueID { get; set; }

        public bool? IsAffectDownline { get; set; }

        public bool IsFromQueue { get; set; }
    }
}
