using System.ComponentModel;

namespace Fanex.NAP.CTS.Model
{
    public enum AccountStatus
    {
        Open = 1,
        Suspended = 2,
        Closed = 3,
        Active = 4,
        ViewOnly = 5,
        Inactive = 6,
        Locked = 7,
        Archived = 8,
        Disable = 9,
        NA = 10
    }

    public enum AssociationType
    {
        Device = 1,
        BettingPattern = 2,
        Manual = 3,
        IP = 4,
        All = 0
    }

    public enum AssociationDetectionType
    {
        Device = 1,
        AI = 2,
        IP = 4
    }

    public enum EvidenceStatus
    {
        All,
        Affected,
        Clean
    }

    public enum AccountEvidenceStatus
    {
        Clean,
        CentralPoint,
        Flagged
    }

    public enum TransactionHistoryLength
    {
        Hours24 = 1,
        Days14 = 14,
        Weeks5 = 35
    }

    public enum ProblemAccountStatus
    {
        Yes,
        No
    }

    public enum RelationshipNodeType
    {
        Start,
        End
    }

    public enum ClassificationSourceType
    {
        NewMember = 201,
        NewMember31 = 231,
        NewMember32 = 232,
        NewMember33 = 233,
        NewMember39 = 239,
        NormalAccount = 202,
        NormalAccount121 = 221,
        NormalAccount122 = 222,
        NormalAccount123 = 223,
        NormalAccountInactive = 1001,
        SmartPunter = 203,
        SmartPunter61 = 261,
        SmartPunter62 = 262,
        SmartPunter63 = 263,
        SmartPunter67 = 267,
        SmartPunter68 = 268,
        HighRiskPunter = 204,
        HighRiskPunter71 = 271,
        HighRiskPunter72 = 272,
        HighRiskPunter73 = 273,
        HighRiskPunter77 = 277,
        HighRiskPunter78 = 278,
        EarlyWarningMembers = 65,
        EarlyWarningMembersLosing = 105,
        GoodMember = 205,
        GoodMember111 = 211,
        GoodMember112 = 212,
        GoodMember113 = 213,
        Probation = 206,
        Probation51 = 251,
        Probation52 = 252,
        Probation53 = 253,
        Probation57 = 257,
        Probation58 = 258,
        RobotUser61 = 61,
        RobotUser163 = 163,
        RobotUser164 = 164,
        RobotOCRD165 = 165,
        RobotOCRD166 = 166,
        Robot81 = 81,
        Robot161 = 161,
        ProPunterInactive = 1002
    }

    public enum SourceTypeID
    {
        Classification_PAMarkDirectly = 31,
        Classification_PAAffectedByUplines = 32,
        Classification_PAUnmarkDirectly = 33,
        Classification_UnmarkingPAAffectedDirectly = 34,
        Classification_ClassifyGBMatchMonitor = 35,
        Classification_Rescan = 36,
        Classification_InsertRobot = 37,
        NoShow_Detail = 38,
        UnauthorizedLogin_Customer = 39
    }

    public enum EvidenceQueueActionID
    {
        Insert = 1,
        Overrride = 2,
        Clear = 3
    }

    public enum ProblemAccountAction
    {
        Mark = 1,
        Unmark = 2
    }

    public enum ActionTypeQueueCustCategory
    {
        Insert = 1,
        Remove = 2
    }

    public enum FromAction
    {
        WebSite = 1,
        Service = 2
    }

    public enum InsertPA_ByType
    {
        Category,
        Reason
    }

    public enum DetectAssociation_QueryType
    {
        DetectAss = 1,
        GetTicketDetail = 2,
        GetAssByGroup = 3,
        GetTicketDetailByListCust = 4
    }

    public enum CustomerCategoryGroup
    {
        RobotUser = 163
    }

    public enum CustomerCategoryID
    {
        VVIP = 210,
        SystemDetectGroupBetting = 63,
        RobotOCRD = 165,
        SysDetectUnauthorizedLogin = 64,
        SysDetectUnauthorizedLoginLosing = 104
    }

    public enum IPPatternType3S
    {
        None = 0,
        IPShared3Days = 1,
        AtLeast3IPsSameDay = 2,
        IPAlternative = 3
    }

    public enum AccountAssociationByAI_3G5SPattern
    {
        Stake = 1,
        IP = 2
    }

    public enum AccountAssociation_ByAIPattern
    {
        Pattern_3G5S = 2
    }

    public enum AccountAssociationByOTGB
    {
        CheckExist = 0,
        Round1 = 1,
        Round2 = 2,
        Round3 = 3
    }

    public enum StatusTicket
    {
        Reject = 101,
        Void = 102,
        Running = 0,
        Waiting = 1,
        Won = 112,
        Draw = 111,
        Lose = 113,
        Refund = 103,
        Completed = 131
    }

    public enum StaticListID
    {
        Reason = 16,
        Sport = 17
    }

    public enum BetTypeSettingsFunctionID
    {
        MMBetType = 1,
        DgrBetType = 2
    }

    public enum StatusCashout
    {
        [Description("All")]
        All = -1,

        [Description("Non-Cashout")]
        NonCashout = 0,

        [Description("Cashout")]
        Cashout = 1,
    }
}