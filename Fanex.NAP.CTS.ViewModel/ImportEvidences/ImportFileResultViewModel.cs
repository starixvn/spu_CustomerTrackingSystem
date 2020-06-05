namespace Fanex.NAP.CTS.ViewModel.ImportEvidences
{
    using System.Collections.Generic;

    public class ImportFileResultViewModel
    {
        public string FileName { get; set; }

        public IEnumerable<RuleCheckViewModel> CheckRuleResultList { get; set; }
    }
}