namespace Fanex.NAP.CTS.Service.Interface.Api
{
    using Fanex.NAP.CTS.Model.API.TWSpecialLicSub;

    public interface ITWSpecialLicSubService
    {
        InsertTWSpecialLicSubEntity InsertTWSpecialLicSub(ForceTWLicSubCCModel model);
    }
}
