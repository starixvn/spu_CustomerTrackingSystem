namespace Fanex.NAP.CTS.ViewModel.Account
{
    using System;
    using System.Collections.Generic;

    public class AssociationByIPInfoViewModel
    {
        public AssociationByIPInfoViewModel()
        {
            IPMatchBettypeList = new List<AssByIPMatchBettypeViewModel>();
            IPTicketStakeList = new List<AssByIPTicketStakeViewModel>();
        }

        public IEnumerable<AssByIPMatchBettypeViewModel> IPMatchBettypeList { get; set; }

        public IEnumerable<AssByIPTicketStakeViewModel> IPTicketStakeList { get; set; }

        public string ErrorMessage { get; set; }

        public int ErrorCode
        {
            get
            {
                return string.IsNullOrEmpty(ErrorMessage) ? 0 : -1;
            }
        }
    }
}
