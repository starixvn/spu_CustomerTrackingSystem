namespace Fanex.NAP.CTS.ViewModel.ProblemAccountManagement
{
    using System;   

    public class ProblemAccountFilterViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Sites { get; set; }

        public string UserName { get; set; }

        public bool HasDownline { get; set; }

    }
}
