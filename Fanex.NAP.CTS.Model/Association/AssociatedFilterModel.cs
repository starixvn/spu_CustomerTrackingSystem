namespace Fanex.NAP.CTS.Model.Association
{
    public class AssociatedFilterModel
    {
        public int draw { get; set; }

        public int start { get; set; }

        public int length { get; set; }

        public long CTSCustId { get; set; }

        public string StatusIds { get; set; }

        public int AssociationType { get; set; }

        public bool IsDirect { get; set; }
    }
}
