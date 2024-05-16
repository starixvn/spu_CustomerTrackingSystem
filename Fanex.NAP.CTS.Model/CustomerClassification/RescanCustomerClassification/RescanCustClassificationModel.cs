namespace Fanex.NAP.CTS.Model.RescanCustClassificationModel
{
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class RescanCustClassificationModel
    {
        public RescanCustClassificationModel()
        {
            IsUpdateDangerToMainDB = true;
            IsPushCCToSoon88 = true;
            IsToQueue = false;
        }
      
        public IEnumerable<long> CustIds { get; set; }

        public bool IsRescanPotentialPA { get; set; }

        public bool IsRescanPA { get; set; }

        public bool IsUpdateDangerToMainDB { get; set; }

        public bool IsPushCCToSoon88 { get; set; }

        public bool IsToQueue { get; set; }

        public FunctionId FunctionId { get; set; }

        public string ServiceName { get; set; }

        public FunctionId FuncId_CheckRobotUser
        {
            get
            {
                switch (FunctionId)
                {
                    case FunctionId.InsertNewCustJob:
                        return FunctionId.InsertNewCus_CheckRobotUser;

                    case FunctionId.RemoveVIPWeb:
                        return FunctionId.RemoveVVIP_CheckRobotUser;

                    case FunctionId.RemoveSpecialCCWeb:
                        return FunctionId.RemoveSpecial_CheckRobotUser;

                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        /// True for removing specialCC
        /// </summary>
        public bool IsDataChanged
        {
            get
            {
                if (FunctionId == FunctionId.RemoveSpecialCCWeb)
                {
                    return true;
                }

                return false;
            }
        }
    }
}