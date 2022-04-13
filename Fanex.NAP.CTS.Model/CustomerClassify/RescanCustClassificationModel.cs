namespace Fanex.NAP.CTS.Model.RescanCustClassificationModel
{
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class RescanCustClassificationModel
    {
        public IEnumerable<long> CustIds { get; set; }

        public bool IsCheckPAFromNAP { get; set; }

        public bool IsPushCCToSoon88 { get; set; }

        public FunctionId FunctionId { get; set; }

        public bool IsUpdateDangerToMainDB { get; set; }

        public DangerLevel DangerLevelType { get; set; }

        public FunctionId FuncId_CheckPA
        {
            get
            {
                switch (FunctionId)
                {
                    case FunctionId.InsertNewCustJob:
                        return FunctionId.InsertNewCus_CheckPA;

                    case FunctionId.RemoveVIPWeb:
                        return FunctionId.RemoveVVIP_CheckPA;

                    case FunctionId.RemoveSpecialCCWeb:
                        return FunctionId.RemoveSpecial_CheckPA;

                    case FunctionId.RemovePAAPI:
                        return FunctionId.RemovePA_CheckPA;

                    case FunctionId.RemoveFlaggedEvidenceWeb:
                        return FunctionId.RemoveFlaggedEvidence_CheckPA;

                    case FunctionId.RemoveMultiEvidencesWeb:
                        return FunctionId.RemoveMultiEvidences_CheckPA;

                    case FunctionId.ExcludeTaggingAssPAWeb:
                        return FunctionId.ExcludeTaggingByAss_CheckPA;

                    default:
                        return 0;
                }
            }
        }

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

                    case FunctionId.RemovePAAPI:
                        return FunctionId.RemovePA_CheckRobotUser;

                    case FunctionId.RemoveFlaggedEvidenceWeb:
                        return FunctionId.RemoveFlaggedEvi_CheckRobotUser;

                    case FunctionId.RemoveMultiEvidencesWeb:
                        return FunctionId.RemoveMultiEvi_CheckRobotUser;

                    case FunctionId.ExcludeTaggingAssPAWeb:
                        return FunctionId.ExcludeTaggingAss_CheckRobotUser;

                    default:
                        return 0;
                }
            }
        }
    }
}