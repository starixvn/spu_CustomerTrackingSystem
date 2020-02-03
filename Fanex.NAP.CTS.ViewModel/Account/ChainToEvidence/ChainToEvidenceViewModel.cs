namespace Fanex.NAP.CTS.ViewModel.Account
{
    using Fanex.NAP.CTS.Model.Account;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class ChainToEvidenceViewModel
    {
        public ChainToEvidenceViewModel()
        {
            FlaggedEvidenceList = new List<FlaggedEvidenceItem>();
        }

        public List<FlaggedEvidenceItem> FlaggedEvidenceList { get; set; }

        public void AddFlaggedEvidenceList(IEnumerable<FlaggedEvidenceModel> flaggedEvidenceModels)
        {
            foreach (var item in flaggedEvidenceModels)
            {
                var flaggedEvidenceItem = new FlaggedEvidenceItem(item);
                this.FlaggedEvidenceList.Add(flaggedEvidenceItem);
            }
        }
    }
}