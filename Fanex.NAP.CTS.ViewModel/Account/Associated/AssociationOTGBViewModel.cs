
namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using Fanex.NAP.CTS.Model.Association;
    using System.Collections.Generic;
    using System.Linq;

    public class AssociationOTGBViewModel
    {
        public AssociationOTGBViewModel()
        {
            AssociationOTGBRoun1 = Enumerable.Empty<AssociationOTGBModel>();
            AssociationOTGBRoun2 = Enumerable.Empty<AssociationOTGBModel>();
            AssociationOTGBRoun3 = Enumerable.Empty<AssociationOTGBModel>();
            ScoreRound1 = false;
            ScoreRound2 = false;
            ScoreRound3 = false;
        }

        public IEnumerable<AssociationOTGBModel> AssociationOTGBRoun1 { get; set; }

        public IEnumerable<AssociationOTGBModel> AssociationOTGBRoun2 { get; set; }

        public IEnumerable<AssociationOTGBModel> AssociationOTGBRoun3 { get; set; }

        public bool ScoreRound1 { get; set; }

        public bool ScoreRound2 { get; set; }

        public bool ScoreRound3 { get; set; }
    }
}