using System;

namespace Fanex.NAP.CTS.Model.Account
{
    public class CustomerPoliciesModel
    { 
        public decimal? MaxPerMatch { get; set; }

        public decimal? MaxBet { get; set; }

        public bool IsVIP { get; set; }

        public int?  CustomerClass { get; set; }

        public string BetLimitToString {
            get
            {
                if (MaxPerMatch != null && MaxBet != null)
                    return  MaxBet?.ToString("N0") + " / " + MaxPerMatch?.ToString("N0");
                else
                    return " ";
            }
        }

        public string CustomerClassToString
        {
            get
            {
                return (CustomerClass != null) ? CustomerClass.ToString() : " ";
            }
        }

        public string LicVip
        {
            get
            {
                return IsVIP ? "Yes" : "No"; 
            }
        }
    }
}