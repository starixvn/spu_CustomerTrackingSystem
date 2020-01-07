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
        NotYetDeposit = 10,
        NA = 11
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
        Days7 = 7,
        Days30 = 30,
        Days180 = 180
    }

    public enum ProblemAccountStatus
    {
        Yes,
        No
    }
}