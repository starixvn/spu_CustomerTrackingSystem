namespace Fanex.NAP.CTS.Model.Account
{
    using System.Collections.Generic;

    public class MatchesAssociationByAIModel
    {
        public IEnumerable<GBAssociationAIModel> MatchesInfo{ get; set; }

        public int TotalMatchCount { get; set; }

        public string ErrorMessage { get; set; }

        public int ErrorCode { get; set; }
    }
}
