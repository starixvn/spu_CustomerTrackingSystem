namespace Fanex.NAP.CTS.Service
{    
    using System.Collections.Generic;

    public interface IRescanCustomerClassifyService
    {
        bool RescanCustomerClassify(List<int> custIds);        
    }
}
