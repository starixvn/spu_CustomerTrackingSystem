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
#pragma warning disable CS0168 // The variable 'maintainceFrom' is declared but never used
            DateTime maintainceFrom;
#pragma warning restore CS0168 // The variable 'maintainceFrom' is declared but never used
#pragma warning disable CS0168 // The variable 'maintainceTo' is declared but never used
            DateTime maintainceTo;
#pragma warning restore CS0168 // The variable 'maintainceTo' is declared but never used

            if (maintenanceInfo.IsUnderMaintenanceTime
                /*&& DateTime.TryParse(maintenanceInfo.From, out maintainceFrom)
                && DateTime.TryParse(maintenanceInfo.To, out maintainceTo)
                && currentTime >= maintainceFrom
                && currentTime <= maintainceTo*/)
            {
                return true;
            }

            return false;
        }
    }
}
