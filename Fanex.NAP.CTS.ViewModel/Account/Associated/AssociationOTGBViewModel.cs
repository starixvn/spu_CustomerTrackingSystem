
namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using Fanex.NAP.CTS.Model.Association;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public class AssociationOTGBViewModel
    {
        public AssociationOTGBViewModel()
        {
            AssociationOTGBRoun1 = Enumerable.Empty<AssociationOTGBModel>();
            AssociationOTGBRoun2 = Enumerable.Empty<AssociationOTGBModel>();
            AssociationOTGBRoun3 = Enumerable.Empty<AssociationOTGBModel>();
        }

        public IEnumerable<AssociationOTGBModel> AssociationOTGBRoun1 { get; set; }

        public IEnumerable<AssociationOTGBModel> AssociationOTGBRoun2 { get; set; }

        public IEnumerable<AssociationOTGBModel> AssociationOTGBRoun3 { get; set; }

    }
}