namespace Fanex.NAP.CTS.Service.Implement.Api
{
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Service.Interface.Api;

    public class LicenseeVIPBAService : ILicenseeVIPBAService
    {
        public bool CheckValid(LicenseeVIPBAInputModel parameter)
        {
            if (parameter == null || parameter.CreatedBy == 0 || string.IsNullOrWhiteSpace(parameter.CustIDs))
            {
                return false;
            }

            return true;
        }

        public bool CheckValid(RemoveLicenseeBABySiteInputModel parameter)
        {
            if (parameter == null || parameter.CreatedBy == 0 || string.IsNullOrWhiteSpace(parameter.SiteName))
            {
                return false;
            }

            return true;
        }
    }
}