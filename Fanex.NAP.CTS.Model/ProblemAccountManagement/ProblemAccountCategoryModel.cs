namespace Fanex.NAP.CTS.Model.ProblemAccountManagement
{
    public class ProblemAccountCategoryModel
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public int CategoryGroup { get; set; }

        public int RelevantCategoryGroup { get; set; }

        public int Priorty { get; set; }

        public bool IsRobotUser { get; set; }

        public bool IsRobotOCRD { get; set; }

        public bool IsSystemDetect { get; set; }

    }
}
