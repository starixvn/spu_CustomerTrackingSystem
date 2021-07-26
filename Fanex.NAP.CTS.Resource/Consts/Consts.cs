namespace Fanex.NAP.CTS.Resource.Consts
{
    public static class Consts
    {
        public const string BaseController = "BaseController";
        public const string MenuItemHome = "Home";
        public const string MenuItemController = "Controller";
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

        public const string FormatDateToString_MMMddyyyy = "MMM dd, yyyy";
        public const string FormatDateToString_MMMddyyyy2 = "MMM-dd-yyyy";
        public const string FormatDateToString_MMMddyyyyhhmmss = "MMM dd, yyyy HH:mm:ss";
        public const string FormatDateToString_MMMddyyyyhhmmsstt = "MMM dd, yyyy HH:mm:ss tt";
        public const string FormatDateToString_MMMddyyyyhhmm = "MMM dd, yyyy HH:mm";
        public const string FormatDateToString_yyyyMMddHHmmss = "yyyyMMddHHmmss";
        public const string FormatDateTimeToString_YYYYmmddThhddmm = "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'";
        public const string FormatDateTimeToString_YYYYmmdd = "yyyyMMdd";
        public const string FormatDecimalToString_2Places = "0.00";
        public const string FormatDecimalToString_Thousands2Places = "#,##0.00";

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

        public const int MatchMonitor_ReportQueryType = 2;
        
        public const int ProblemCase_ItemID_Robot = 1;
        public const int ProblemCase_ItemID_SuspiciousAssociation = 4;
        public const int ProblemCase_ItemID_ProPunter = 9;

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

        public enum MATCHMONITOR_BETTYPE
        {
            HDP = 1,
            OU = 3,
            FirstHDP = 7,
            FirsttOU = 8
        }

        public enum MATCHMONITOR_EVENTGROUP
        {
            Live = 1,
            Today = 2,
            Early = 3,
            Closed = 4
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

        public enum FunctionId
        {
            RealtimeJob = 1,
            ProbationJob = 2,
            DailyJob = 3,
            InsertNewCustJob = 4,
            RemoveVIPWeb = 5,
            RemoveSpecialCCWeb = 6,
            RemovePAAPI = 7,
            RemoveFlaggedEvidenceWeb = 8,
            RemoveMultiEvidencesWeb = 9,
            ExcludeTaggingAssPAWeb = 10,
            PlaceEvidenceForMultiCustAPI = 21,
            PlaceMultiEvidencesForMultiCustAPI = 22,
            RobotUserJob = 23,
            RescanPAFromNAPPA = 24,
            RescanRobotUser = 25,
            InsertPAJob = 26
        };
    }
}