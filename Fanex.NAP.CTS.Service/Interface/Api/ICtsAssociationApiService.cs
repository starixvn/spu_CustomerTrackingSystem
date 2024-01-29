namespace Fanex.NAP.CTS.Service.Interface.Api
{
    using Fanex.NAP.CTS.Model.API;
    using System.Collections.Generic;

    public interface ICtsAssociationApiService
    {
        IEnumerable<DetectAssociationByTypeModel> DetectAssociationByCustId(long custID);
    }
}
