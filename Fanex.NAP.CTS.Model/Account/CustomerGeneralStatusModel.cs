namespace Fanex.NAP.CTS.Model.Account
{
    public class CustomerGeneralStatusModel
    {
        public int custid { get; set; }

        public string username { get; set; }

        /// <summary>
        /// 1: Inactive, 2: Suspended, 3: ViewOnly, 4: Active
        /// -1: customer isn't CN88
        /// </summary>
        public int cn88status { get; set; }

        /// <summary>
        /// suspend (reject=true) only for MBC
        /// </summary>
        public bool reject { get; set; }

        /// <summary>
        /// CN88: locked (closed=1) || MBC: disabled (closed=1) active (closed=0)
        /// </summary>
        public bool closed { get; set; }

        public int danger { get; set; }

        public int danger2 { get; set; }

        public int danger3 { get; set; }

        public bool autosuspend { get; set; }

        public bool autosuspend2 { get; set; }

        public int Abnormal { get; set; }

        public bool ProblemAccount { get; set; }
    }
}
