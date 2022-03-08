namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.ProbationManagement;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IProblemAccountManagementService
    {
        IEnumerable<CategoryModel> GetProblemAccountCategory();
    }
}
