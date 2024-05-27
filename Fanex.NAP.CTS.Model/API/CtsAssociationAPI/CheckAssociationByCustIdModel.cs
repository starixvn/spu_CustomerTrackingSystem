namespace Fanex.NAP.CTS.Model.API
{
    public class CheckAssociationByCustIdModel
    {
        public CheckAssociationByCustIdModel ()
        {
            Status = 200;
        }

        public bool IsAssociated { get; set; }

        public string ErrMessage { get; set; }

        public int Status { get; set; }
    }
}
