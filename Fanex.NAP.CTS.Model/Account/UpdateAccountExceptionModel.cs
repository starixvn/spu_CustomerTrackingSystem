namespace Fanex.NAP.CTS.Model.Account
{
    public class UpdateAccountExceptionModel
    {
        public int UserId { get; set; }

        public int SubscriberId { get; set; }

        public long FromCTSCustId { get; set; }

        public long ToCTSCustId { get; set; }

        public string ToUserName { get; set; }

        public string Comment { get; set; }
    }
}
