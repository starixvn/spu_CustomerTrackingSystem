namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    using System.Collections.Generic;

    public class DangerViewModel
    {
        public DangerViewModel()
        {
            this.Danger1 = new DangerTypeViewModel();
            this.Danger2 = new DangerTypeViewModel();
            this.Danger3 = new DangerTypeViewModel();
            this.Danger4 = new DangerTypeViewModel();
            this.Danger5 = new DangerTypeViewModel();
            this.NoneDanger = new DangerTypeViewModel();
        }

        public DangerTypeViewModel Danger1 { get; set; }

        public DangerTypeViewModel Danger2 { get; set; }

        public DangerTypeViewModel Danger3 { get; set; }

        public DangerTypeViewModel Danger4 { get; set; }

        public DangerTypeViewModel Danger5 { get; set; }

        public DangerTypeViewModel NoneDanger { get; set; }
    }
}
