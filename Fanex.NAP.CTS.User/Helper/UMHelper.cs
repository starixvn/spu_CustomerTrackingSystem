using ONEbook.UM.Services;
using System;

namespace Fanex.NAP.CTS.User.Helper
{
    public static class UMHelper
    {
        public static bool IsUnderMaintainaceHappen(IMaintenanceService maintenanceService)
        {
            var maintenanceInfo = maintenanceService.GetUnderMaintenanceTime();
            //GMT+8 Taiwan
            var currentTime = DateTime.UtcNow.AddHours(8);
            DateTime maintainceFrom;
            DateTime maintainceTo;

            if (maintenanceInfo.IsUnderMaintenanceTime
                && DateTime.TryParse(maintenanceInfo.From, out maintainceFrom)
                && DateTime.TryParse(maintenanceInfo.To, out maintainceTo)
                && currentTime >= maintainceFrom
                && currentTime <= maintainceTo)
            {
                return true;
            }

            return false;
        }
    }
}
