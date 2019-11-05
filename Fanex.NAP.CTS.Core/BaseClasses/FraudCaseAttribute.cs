namespace Fanex.NAP.CTS.Core.BaseClasses
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class FraudCaseAttribute : Attribute
    {
        public FraudCaseAttribute(FraudCaseId fraudCase)
        {
            FraudCase = fraudCase;
        }

        public FraudCaseId FraudCase { get; }
    }

    public enum FraudCaseId
    {
        BSNG = 1,
        FlaggedAccount = 6,
        DelayedGoal = 8,
        ScannedOfficeIP = 2,
        DailyDataCapture = 4,
        CheckBet = 3,
        Arbitrage = 10,
        BetsNearGoal = 11
    }
}