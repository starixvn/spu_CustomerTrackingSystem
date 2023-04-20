namespace Fanex.NAP.CTS.Model.Account
{   
    using System.Collections.Generic;    

    public class AssociationByIPInfoModel
    {           
        public string ErrorMessage { get; set; }

        public IEnumerable<AssociationByIPItemModel> ItemList { get; set; }
    }
}
