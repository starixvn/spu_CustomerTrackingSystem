namespace Fanex.NAP.CTS.ViewModel.Account.CustomerInfo
{
    using System;

    public class CustomerInfoDateViewModel
    {
        public CustomerInfoDateViewModel() { }

        public DateTime NextCategoryScan { get; set; }

        public string NextCategoryScanString {
            get
            {
                if (this.NextCategoryScan == DateTime.MinValue)
                {
                    return "None";
                }

                return this.NextCategoryScan.ToString("MMM dd, yyyy");
            } 
        }

        public DateTime LastTicket { get; set; }

        public string LastTicketString
        {
            get
            {
                if (this.LastTicket == DateTime.MinValue)
                {
                    return "None";
                }

                return this.LastTicket.ToString("MMM dd, yyyy HH:mm");
            }
        }

        public DateTime LastLogin { get; set; }

        public string LastLoginString
        {
            get
            {
                if (this.LastLogin == DateTime.MinValue)
                {
                    return "None";
                }

                return this.LastLogin.ToString("MMM dd, yyyy HH:mm");
            }
        }

        public DateTime Created { get; set; }

        public string CreatedString 
        {
            get
            {
                if (this.Created == DateTime.MinValue)
                {
                    return "None";
                }

                return this.Created.ToString("MMM dd, yyyy HH:mm");
            }
        }
    }
}
