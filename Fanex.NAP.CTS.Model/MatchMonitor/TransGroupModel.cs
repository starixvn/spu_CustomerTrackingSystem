namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System;

    public class TransGroupModel
    {
        public Int64 TransId { get; set; }

        public int GroupName { get; set; }

        public string VerifiedDate { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        public string VerifiedBy { get; set; }
    }
}
