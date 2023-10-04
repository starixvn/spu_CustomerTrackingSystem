namespace Fanex.NAP.CTS.Resource.Consts
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public static class Consts
    {
        public const string BaseController = "BaseController";
        public const string MenuItemHome = "Home";
        public const string MenuItemController = "Controller";
        public const string MenuItemMonitor = "Monitor";
        public const string MenuItemSearch = "Account & Device Search";
        public const string MenuItemSuspiciousBets = "Suspicious Bets";
        public const string MenuItemReport = "Report";
        public const string MenuItemSettings = "Settings";

        public const string SearchAccountKey = "search-account";
        public const string SearchDeviceKey = "search-device";
        public const string SearchAccountOption = "Search account";
        public const string SearchDeviceOption = "Search device";

        public const string GraphDBGetAssociationOneLevel = "GraphDB_GetAssociationOneLevel";
        public const string GraphDBGetAssociationNLevel = "GraphDB_GetAssociationNLevel";
        public const string GraphDBGetShortestPath = "GraphDB_GetShortestPath";

        public const string FormatDateToString_ddMMhhmmss = "dd/MM HH:mm:ss";
        public const string FormatDateToString_MMMddyyyy = "MMM dd, yyyy";
        public const string FormatDateToString_MMMddyyyy2 = "MMM-dd-yyyy";
        public const string FormatDateToString_ddMMMyyyy = "dd MMM, yyyy";
        public const string FormatDateToString_MMMddyyyyhhmmss = "MMM dd, yyyy HH:mm:ss";        
        public const string FormatDateToString_MMMddyyyyhhmmss2 = "MMM dd, yyyy - HH:mm:ss";
        public const string FormatDateToString_MMMddyyyyhhmmsstt = "MMM dd, yyyy HH:mm:ss tt";
        public const string FormatDateToString_MMMddyyyyhhmm = "MMM dd, yyyy HH:mm";
        public const string FormatDateToString_yyyyMMddHHmmss = "yyyyMMddHHmmss";
        public const string FormatDateTimeToString_YYYYmmddThhddmm = "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'";
        public const string FormatDateTimeToString_YYYYmmdd = "yyyyMMdd";        
        public const string FormatDatetimeToString_ddMMMyyyyHHmmssfff = "dd MMM, yyyy HH:mm:ss.fff";
        public const string FormatDateTimeToString_ddMMyyyy = "dd/MM/yyyy";
        public const string FormatDateTimeToString_ddMMyyyyhhmmss = "dd/MM/yyyy HH:mm:ss";
        public const string FormatDatetimeToString_YYYYmmddHHmmss = "yyyy-MM-dd HH:mm:ss";
        public const string FormatDatetimeToString_ddMMMHHmmssfff = "dd MMM HH:mm:ss.fff";
        public const string FormatDatetimeToString_ddMMM = "dd MMM";
        public const string FormatTimeToString_hhmmss = "HHmmss";
        public const string FormatTimeToString_hhmmss_2 = "HH:mm:ss";
        public const string FormatDecimalToString_2Places = "0.00";
        public const string FormatDecimalToString_Thousands = "#,##0";
        public const string FormatDecimalToString_Thousands2Places = "#,##0.00";
        public const string FormatDateToString_MMMddhhmmss = "MMM dd HH:mm:ss";
        public const string FormatDateTimeToString_yyyyMMdd = "yyyy/MM/dd";
        public const string FormatDateTimeToString_yyyyMM = "yyyy/MM";

        public const string CurrentSubscriberAssignmentsKey = "CurrentSubscriberAssignmentsKey";
        public const string IsMasterSubscriberAssignmentKey = "IsMasterSubscriberAssignmentKey";
        public const string IsPermissionCN88Key = "IsPermissionCN88Key";
        public const string SubscriberNameCN88 = "CN88";
        public const string SettingEnvironmentKey = "Environment";
        public const string SettingEnvironmentLocalValue = "1";
        public const string SettingSiteIdKey = "SiteId";
        public const string SettingSiteIdValue = "108";

        public const string INVALID_PARAMETERS_ERROR = "Invalid parameter";

        public const int VALID_INPUT = 0;
        public const int DUPLICATED_INPUT = 1;
        public const int DUPLICATED_DB = 2;
        public const int NOT_EXIST = 3;
        public const int INVALID_INPUT = 4;

        public const string IsFirstGetDataNotification = "false";
        public const int ErrorMess_LosingFuncNotificaionSetting = -98;
        public const int ErrorMess_LosingAllFuncNotificaionSetting = -99;

        public const int SUPER_ROLE = 4;
        public const int MASTER_ROLE = 3;
        public const int AGENT_ROLE = 2;

        public const int REMOVE_PROBLEMACCOUNT_CATEGORYID = -1;

        public const string SALT = "!%HGDwweHqw#";
        public const string AssociationTypeName_BettingPattern = "Betting Pattern";
        public const string AssociationTypeName_IP = "IP";
        public const string AssociationTypeName_IPAssociation = "IP Association";

        public const int ProblemCase_ItemID_Robot = 1;
        public const int ProblemCase_ItemID_SuspiciousAssociation = 4;
        public const int ProblemCase_ItemID_ProPunter = 9;

        public const int Retry = 3;

        public const string MatchMonitorBetType_HDP = "HDP";
        public const string MatchMonitorBetType_OU = "O/U";
        public const string MatchMonitorBetType_1stHDP = "1st HDP";
        public const string MatchMonitorBetType_1stOU = "1st O/U";
        public const string MatchMonitorBetType_Moneyline = "Moneyline";
        public const string MatchMonitorBetType_QuarterXHDP = "Quarter X HDP";
        public const string MatchMonitorBetType_QuarterXOU = "Quarter X O/U";
        public const string MatchMonitorBetType_QuarterXMoneyline = "Quarter X Moneyline";
        public const string MatchMonitorBetType_Quarter1HDP = "Quarter 1 HDP";
        public const string MatchMonitorBetType_Quarter2HDP = "Quarter 2 HDP";
        public const string MatchMonitorBetType_Quarter3HDP = "Quarter 3 HDP";
        public const string MatchMonitorBetType_Quarter4HDP = "Quarter 4 HDP";
        public const string MatchMonitorBetType_Quarter1OU = "Quarter 1 O/U";
        public const string MatchMonitorBetType_Quarter2OU = "Quarter 2 O/U";
        public const string MatchMonitorBetType_Quarter3OU = "Quarter 3 O/U";
        public const string MatchMonitorBetType_Quarter4OU = "Quarter 4 O/U";
        public const string MatchMonitorBetType_Quarter1Moneyline = "Quarter 1 Moneyline";
        public const string MatchMonitorBetType_Quarter2Moneyline = "Quarter 2 Moneyline";
        public const string MatchMonitorBetType_Quarter3Moneyline = "Quarter 3 Moneyline";
        public const string MatchMonitorBetType_Quarter4Moneyline = "Quarter 4 Moneyline";
        public const string ProblemAccountManagement_DuplicatedUserName = "Duplicated Username";
        public const string ProblemAccountManagement_SiteLicensee = "Licensee";
        public const string ProblemAccountManagement_SiteCredit = "Credit";
        public const string ProblemAccountManagement_CategoryPA = "Problem Account Category";
        public const string MarkPA_AssociatedGroup = "Associated Group";

        public struct Platform
        {
            public const string Desktop = "Desktop";
            public const string Mobile = "Mobile";
        }

        public struct MMSites
        {
            public const string All = "A";
            public const string Credit = "C";
            public const string Licensee = "L";
        }

        public enum ROLE_CUSTOMER
        {
            MEMBER = 1,
            AGENT = 2,
            MASTER = 3,
            SUPER = 4
        }

        public enum CATEGORY
        {
            NEWMEMBER = 201,
            NORMALMEMBER = 202,
            SMARTPUNTER = 203,
            HIGHRISKPUNTER = 204,
            VIP = 205,
            PROBATION = 206,
            VVIP = 213,
            ROBOT = 61
        };

        public enum ACCOUNT_STATUS
        {
            All = 0,
            Open = 1,
            Closed = 2,
            Other = 3
        }

        public enum MatchMonitorBetType
        {
            HDP = 1,
            OE = 2,
            OU = 3,
            TotalGoal = 6,
            FirstHDP = 7,
            FirstOU = 8,
            FirstOE = 12,
            Moneyline = 20,
            FirstTotalGoal = 126,
            CorrectScore = 413,
            FirstCorrectScore = 414,
            QuarterXHDP = 609,
            QuarterXOU = 610,
            QuarterXMoneyline = 612
        }

        public enum MatchMonitorBetId
        {
            first = 801,
            second = 802,
            third = 803,
            fourth = 804
        }

        public enum MATCHMONITOR_SECTION
        {
            Live = 1,
            Today = 2,
            Early = 3,
            Closed = 4
        }

        public enum MatchMonitorBetChoiceId
        {
            All = 0,
            H = 1,
            A = 2,
            O = 3,
            U = 4
        }

        public enum MatchMonitorSportType
        {
            Soccer = 1,
            Basketball = 2,
            ESport = 43,
        }

        public enum MatchMonitorReason
        {
            GroupBetting = 0,
            Hedging = 1,
            Arbitrage = 2,
            FixedGame = 3,
        }

        public enum STATIC_LIST
        {
            PROBLEMCASE = 12
        }

        public enum PROBLEM_CASES_MONITOR
        {
            EXPORT_NUMBER_COLUMN = 6,
            EXPORT_NUMBER_CHAR_PER_LINE = 180,
            EXPORT_POINT_PIXEL = 225
        }

        public enum ROBOT_REPORT
        {
            ROBOT_EXPORT_NUMBER_COLUMN = 5,
            AIROBOT_EXPORT_NUMBER_COLUMN = 6,
            EXPORT_NUMBER_CHAR_PER_LINE = 180,
            EXPORT_POINT_PIXEL = 300
        }

        public enum FunctionId
        {
            RealtimeJob = 1,
            ProbationJob = 2,
            DailyJob = 3,
            InsertNewCustJob = 4,
            RemoveVIPWeb = 5,
            RemoveSpecialCCWeb = 6,
            //RemovePAAPI = 7,
            //RemoveFlaggedEvidenceWeb = 8,
            //RemoveMultiEvidencesWeb = 9,
            ExcludeTaggingAssPAWeb = 10,
            InactivateNewCustomer = 11,
            ProblemDailyJob = 12,
            //InsertEvidenceJob = 13,
            //PlaceSingleEvidence = 14,
            //PlaceEvidenceForMultiCustAPI = 21,
            //PlaceMultiEvidencesForMultiCustAPI = 22,
            RobotUserJob = 23,
            //InsertNewCus_CheckPA = 24,
            //RemoveVVIP_CheckPA = 25,
            //RemoveSpecial_CheckPA = 26,
            //RemovePA_CheckPA = 27,
            //RemoveFlaggedEvidence_CheckPA = 28,
            //RemoveMultiEvidences_CheckPA = 29,
            //ExcludeTaggingByAss_CheckPA = 30,
            InsertNewCus_CheckRobotUser = 34,
            RemoveVVIP_CheckRobotUser = 35,
            RemoveSpecial_CheckRobotUser = 36,
            //RemovePA_CheckRobotUser = 37,
            //RemoveFlaggedEvi_CheckRobotUser = 38,
            //RemoveMultiEvi_CheckRobotUser = 39,
            //ExcludeTaggingAss_CheckRobotUser = 40,
            InsertPAJob = 41,
            //UpdateCategory = 42,?
            InsertPAM = 43,
            TWProblemAccountInsert = 44,
            UnmarkCustomerCategory = 45,
            AddLicVIP_API = 46,
            RemoveLicVIP_API = 47,
            AddLicBA_API = 48,
            RemoveLicBA_API = 49,
            RemoveLicBABySite_API = 50,
        };

        public enum RescanNormalType
        {
            Manual = 4,
            Auto_Customer = 5,
            Auto_AfftectedDownline = 6
        }

        public enum HttpStatus
        {
            Unauthorized = 401,
            BadRequest = 400,
            NotFound = 404,
            MethodNotAllowed = 405,
            OK = 200,
            Created = 201,
            ServerUnavailable = 503,
            InternalServerError = 500
        };

        public enum FromFlow
        {
            CTSWEB = 1,
            CTSAPI = 2,
            CTSServices = 3,
            AFCSerivce = 4,
            ManualTrigger = 5
        };
        
        public enum TrackingCtsFunctionId
        {
            ProblemAccountMonitor = 1,
            EvidenceManagementImport = 2,
            EvidenceManagementRemove = 3,
            AssociatedAccountMonitorRpt = 4,
            AssociatedAccountMonitorInsert = 5,
            AssociatedAccountMonitorRemove = 6,
            SpecialCustomerClassRpt = 7,
            SpecialCustomerClassInsert = 8,
            SpecialCustomerClassRemove = 9,
            ProbationManagement = 10,
            AssociationDetection = 11,
            AssociationReport = 12,
            AssociationDetailRpt = 13,
            AssociationDetailRptLinkedUnlinked = 14,
            LatestTransaction = 15,
            ProblemCaseMonitorRpt = 16,
            ProblemCaseMonitorExport = 17,
            TransactionRpt = 18,
            MatchMonitorRpt = 19,
            MatchMonitorDetailRpt = 20,
            ManageSubscriberRpt = 21,
            ManageSubscriberInsert = 22,
            ManageSubscriberRemove = 23,
            ManageSubscriberUpdate = 24,
            AccountProfileRpt = 25,
            AccountProfileAssAccountRpt = 26,
            AccountProfileAssAccountDetailRpt = 27,
            AccountProfileAssAccountInsert = 28,
            AccountProfileAssAccountViewOnGraph = 29,
            AccountProfileAssAccountLinkedUnlinked = 30,
            AccountProfileChainstoEvidencesRpt = 31,
            //AccountProfileChainstoEvidenceInsertEvidence = 32,
            AccountProfileChainstoEvidenceRemoveEvidence = 33,
            AccountProfileChainstoEvidenceRemoveAffEvidence = 34,
            AccountProfileChainstoEvidenceInsertExceptions = 35,
            AccountProfileChainstoEvidenceRemoveExceptions = 36,
            AccountProfileCustomerClassification = 37,
            AccountProfileCustomerClassificationPinCategory = 38,
            AccountProfileCustomerClassificationNormalCategory = 39,
            AccountProfileCustomerClassificationMarkCategory = 40,
            AccountProfileCustomerClassificationRemoveCategory = 41,
            AccountProfileTransactionHistory = 42,
            AccountProfileRobotClassification = 43,
            AccountProfileRobotBetting = 44,
            Notification = 45,
            NotificationSettingAssAccountMonitor = 46,
            NotificationSettingProbation = 47,
            ProblemAccountManagementRpt = 48,
            ProblemAccountManagementMarkNew = 49,
            AssociatedGroupMonitorRpt = 50,
            AssociationDetectionDetails = 51,
            AccountProfileGetCustomerCategoryBySportRpt = 52,
            AccountProfileGetCustomerCategoryHistoryBySportRpt = 53,
			AssociationDetectionExportToExcel = 54,
  			AssociatedGroupMonitorAddNewGroupRpt = 55,
            AssociatedGroupMonitorUpdateGroupRpt = 56,
            AssociatedGroupMonitorRemoveGroupRpt = 57,
            AssociatedGroupMonitorAddAccountRpt = 58, 
            AssociatedGroupMonitorTransferAccountRpt = 59,
            AssociatedGroupMonitorSetPARpt = 60,
            AssociatedGroupMonitorExportToExcelRpt = 61,
            MatchMonitorDetailManualDetectRpt = 62,
            MatchMonitorDetailExportToExcelRpt = 63,
            MatchMonitorDetailVerifyTransRpt = 64,
            AssociationDetectionByBettingPattern = 65,
            AssociationDetectionByDevice = 66,
            AssociationDetectionByIP = 67,
            AssociationDetectionByAgent = 68,
            AssociationDetectionByMatch7Days = 69,
            AssociationDetectionByIP3Days = 70,
            AssociationDetectionDetailIP = 71,
            AssociationDetectionDetailMatch = 72,
            AssociationDetectionDetailAgent = 73,
            RobotReport = 74,
            RobotReport_ExportToExcel = 75,

            //PlaceEvidenceForMultipleCustAPI = 200,
            GetFlaggedCustomersByLevelAPI = 201,
            GetFlaggedCustByLvAndEvidenceAPI = 202,
            //AddCustomersEvidencesAPI = 203,
            GetAssociationForExtAccAPI = 204,
            AddSystemFormulaBetAccountAPI = 205,
            GetAssociationByCustIdAPI = 206,
            GetCustomerClassificationAPI = 207,
            GetCustomerClassificationByCategoryIDsAPI = 208,
            GetVVIPCustomerAPI = 209,
            CheckCustomerNotExistedAPI = 210,
            //RemoveProblemAccountsAPI = 211,
            //UpdateCategoryToCTSWhenPAUpdatedAPI = 212,
            GetCustomerClassCategoryAPI = 213,
            GetSubscriberInfoAPI = 214,
            GetAssociationPerTypeAPI = 215,
            GetProblemCaseAPI = 216,
            GetProblemCaseMonitorAPI = 217,
            ForceSpecialCCAPI = 218,
            GetDirectAssociationDevicesAPI = 219,
            GetDirectAssociationProfileAPI = 220,
            GetFlaggedEvidenceForLevel01API = 221,
            GetFlaggedEvidenceForLevel2API = 222,
            GetAssociationReportAPI = 223,
            GetAssociationDetectionReportAPI = 224,
            GetStatisticAssociationAPI = 225,
            GetDirectAssociationAPI = 226,
            GetDirectAssociationByManualAPI = 227,
            GetDirectAssociationByDeviceAPI = 228,
            GetDirectAssociationByIPAPI = 229,
            GetDirectAssociationByAIAPI = 230,
            GetDirectAssociationMultiCustAPI = 231,
            GetInDirectAssociationAPI = 232,
            GetShortestPathAPI = 233,
            GroupingCustomerAPI = 234,
            GetDetailAssociationReportAPI = 235,
            GetProbationReportAPI = 236,
            ScanCTSCustomerAPI = 237,
            AddLicVIP_API = 238,
            RemoveLicVIP_API = 239,
            AddLicBA_API = 240,
            RemoveLicBAP_API = 241,
            RemoveLicBABySite_API = 242
        };

        public enum SystemId
        {
            CTSWeb = 1,
            CTSAPI = 2,
            FS = 3,
            TGRS = 4
        };

        public enum FromSystem
        {
            Internal = 1,
            External = 2
        };

        public enum RobotDetectionQueryType
        {
            RobotReasonDetail = 1,
            RobotTicketDetail = 2
        };

        public enum RobotPeriodRangeType
        {
            SCEConsecutive = 100,
            SCEUglyStake = 101,
            OCRD = 200,
            TD = 300
        };

        public enum MatchMonitorQueryType
        {
            TicketFraud = 2,
            TicketAll = 3
        }

        public enum RobotType
        {
            Robot,
            AIRobot
        }

        public enum CustomerCategorySportType
        {
            [Description("Soccer")]
            Soccer = 1,

            [Description("Basketball")]
            Basketball = 2,

            [Description("E-Sports")]
            ESports = 43,

            [Description("Other Sports")]
            OtherSports = 99,

            [Description("Non-Sport")]
            NonSport = 229,

            [Description("Saba Soccer")]
            SabaSoccer = 145,

            [Description("Saba Basketball")]
            SabaBasketball = 912
        }

        public static readonly Dictionary<CustomerCategorySportType, int> CustomerCategorySportOrder = new Dictionary<CustomerCategorySportType, int>
        {
            { CustomerCategorySportType.Soccer          ,     1 },
            { CustomerCategorySportType.Basketball      ,     2 },
            { CustomerCategorySportType.ESports         ,     3 },
            { CustomerCategorySportType.OtherSports     ,     4 },
            { CustomerCategorySportType.NonSport        ,     5 },
            { CustomerCategorySportType.SabaSoccer      ,     6 },
            { CustomerCategorySportType.SabaBasketball  ,     7 },
        };
    }
}