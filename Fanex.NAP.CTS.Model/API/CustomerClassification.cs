using Fanex.NAP.CTS.Core.Helpers.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fanex.NAP.CTS.Model.API
{

    public class CustomerClassification
    {
        public long CustID { get; set; }

        public string ParentCategoryName { get; set; }

        public string CategoryName { get; set; }

    }
}
