
namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    public class CoupleDetailParameterModel
    {
        public string CoupleInfoDetails { get; set; }

        public long CustID1 { get; set; }

        public long CustID2 { get; set; }

        public int PatternType { get; set; }

        public string GroupNamePattern
        {
            get
            {
                switch (PatternType)
                {
                    case 1:
                        return "STAKE PATTERN";
                    case 2:
                        return "IP PATTERN";
                }

                return string.Empty;
            }
        }
    }
}
