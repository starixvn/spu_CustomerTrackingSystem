using Fanex.NAP.CTS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Service.Interface
{
    public interface IClassifyCustomerService
    {
        void ClassifyCustomerGeneralCategoryDaily(string custIds);
    }
}
