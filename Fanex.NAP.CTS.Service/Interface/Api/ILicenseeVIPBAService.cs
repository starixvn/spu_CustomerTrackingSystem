namespace Fanex.NAP.CTS.Service.Interface.Api
{
    using Fanex.NAP.CTS.Model.API;

    public interface ILicenseeVIPBAService
    {
        bool CheckValid(LicenseeVIPBAInputModel parameter);

        void InsertLicVIP(LicenseeVIPBAInputModel parameter);

        void RemoveLicVIP(LicenseeVIPBAInputModel parameter);

        void InsertLicBA(LicenseeVIPBAInputModel parameter);

        void RemoveLicBA(LicenseeVIPBAInputModel parameter);     
    }
}