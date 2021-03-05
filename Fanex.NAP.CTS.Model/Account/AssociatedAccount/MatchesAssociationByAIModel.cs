namespace Fanex.NAP.CTS.Model.Account
{
    using System.Collections.Generic;

    public class MatchesAssociationByAIModel
    {
        public List<MatchesInfoModel> MatchesInfo{ get; set; }

        public int TotalMatchCount { get; set; }

        public string ErrorMessage { get; set; }
    }
}
