namespace Fanex.NAP.CTS.Model.ProblemAccountManagement
{
    public class ProblemAccountCategoryModel
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public int CategoryGroupID { get; set; }

        public int RelevantCategoryGroup { get; set; }

        public int Priorty { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsRobot { get; set; }

        public int RoleGroup { get; set; }
    }
}
