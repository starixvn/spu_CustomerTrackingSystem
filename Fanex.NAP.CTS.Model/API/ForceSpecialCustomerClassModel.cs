
namespace Fanex.NAP.CTS.Model.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ForceSpecialCustomerClassModel
    {
        public string Username { get; set; }

        public int CustomerClass { get; set; }

        public bool IsAffectedToAssociation { get; set; }
    }
}
