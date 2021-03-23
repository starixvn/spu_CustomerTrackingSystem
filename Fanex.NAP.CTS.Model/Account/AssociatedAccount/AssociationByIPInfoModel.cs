namespace Fanex.NAP.CTS.Model.Account
{
    using System;
    using System.Collections.Generic;    

    public class AssociationByIPInfoModel
    {
        public UInt32 TotalMatchCount { get; set; }

        public UInt32 TotalIPCount { get; set; }

        public string ErrorMessage { get; set; }

        public IEnumerable<AssociationByIPItemModel> ItemList { get; set; }
    }
}
