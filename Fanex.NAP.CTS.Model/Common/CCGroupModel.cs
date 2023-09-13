namespace Fanex.NAP.CTS.Model.Common
{
    using System.Collections.Generic;

    public class CCGroupModel
    {
        public CCGroupModel()
        {
            this.CCList = new List<int>();
        }

        public string CCGroupName { get; set; }

        public IEnumerable<int> CCList { get; set; }
    }
}
