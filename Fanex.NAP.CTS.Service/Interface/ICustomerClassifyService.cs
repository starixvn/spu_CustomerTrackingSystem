namespace Fanex.NAP.CTS.Service
{    
    using System.Collections.Generic;

    public interface ICustomerClassifyService
    {
        bool RescanCustomerClassify(List<int> custIds);        
    }
}
