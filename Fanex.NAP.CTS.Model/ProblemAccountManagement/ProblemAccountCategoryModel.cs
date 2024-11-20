using static Fanex.NAP.CTS.Resource.Consts.Consts;

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

        public int RoleID { get; set; }

        public bool MapRole(ROLE_CUSTOMER role)
        {
            var roleGroup = (int)ROLE_CUSTOMER.MEMBER;

            if (role != ROLE_CUSTOMER.MEMBER)
            {
                roleGroup = (int)ROLE_CUSTOMER.AGENT;
            }

            return this.RoleGroup == roleGroup;
        }
    }
}
