namespace Fanex.NAP.CTS.Model.DTO
{
    public class CustomerAssociation
    {
        public int FromCustId { get; set; }

        public int ToCustId { get; set; }

        public string ToOWUsername { get; set; }

        public string CurrencyName { get; set; }
    }
}