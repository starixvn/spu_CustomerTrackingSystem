namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{
    using System.Collections.Generic;

    public class DetectAssociationFilterViewModel
    {              
        public IEnumerable<int> CCList { get; set; }

        public IEnumerable<int> DangerLevel { get; set; }

        public int BotTransSum { get; set; }

        public int BotTransPercentage { get; set; }

        public int InvalidBrowserInfoTransSum { get; set; }

        public int InvalidBrowserInfoTransPercentage { get; set; }
    }
}
