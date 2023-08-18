using System.Collections.Generic;
using Newtonsoft.Json;

namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    public class MMFilterItemViewModel
    {
        public string Score { get; set; }

        public string SubscriberType { get; set; }

        public int Danger1 { get; set; }

        public int Danger2 { get; set; }

        public int Danger3 { get; set; }

        public int Danger4 { get; set; }

        public int Danger5 { get; set; }

        public string DangerToString
        {
            get
            {
                var danger = string.Empty;
                var commaCount = 0;

                if (Danger1 != 0)
                {
                    danger += "OR" + Danger1;
                }

                if (Danger2 != 0)
                {
                    if (string.IsNullOrEmpty(danger))
                    {
                        danger += "A" + Danger2;
                    }
                    else
                    {
                        commaCount++;

                        danger += "," + AddSpaceDanger(commaCount) + "A" + Danger2;
                    }
                }

                if (Danger3 != 0)
                {
                    if (string.IsNullOrEmpty(danger))
                    {
                        danger += "OS" + Danger3;
                    }
                    else
                    {
                        commaCount++;

                        danger += "," + AddSpaceDanger(commaCount) + "OS" + Danger3;
                    }
                }

                if (Danger4 != 0)
                {
                    if (string.IsNullOrEmpty(danger))
                    {
                        danger += "B" + Danger4;
                    }
                    else
                    {
                        commaCount++;

                        danger += "," + AddSpaceDanger(commaCount) + "B" + Danger4;
                    }
                }

                if (Danger5 != 0)
                {
                    if (string.IsNullOrEmpty(danger))
                    {
                        danger += "E" + Danger5;
                    }
                    else
                    {
                        commaCount++;

                        danger += "," + AddSpaceDanger(commaCount) + "E" + Danger5;
                    }
                }

                return (danger == string.Empty) ? "-" : danger;
            }
        }

        public string CurrencyName { get; set; }

        public short CurrencyId { get; set; }

        public string ReasonItem { get; set; } = "-";

        public string ADetectGroup { get; set; }

        public int ADetectGroupID { get; set; }

        private string AddSpaceDanger(int commaCount)
        {
            return commaCount % 2 == 0 ? " " : string.Empty;
        }
    }
}
