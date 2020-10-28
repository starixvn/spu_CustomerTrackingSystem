namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.AssociationReport;
    using Fanex.NAP.CTS.ViewModel.AssociationReport;
    using System.Collections.Generic;

    public interface IAssociationReportService
    {
        IEnumerable<AssociationModel> GetAssociations(AssociationFilterViewModel filter);

        IEnumerable<CategoriesModel> GetCategories(string ctsCustIdList);

        IEnumerable<AssociationDetailModel> GetAssociationDetail(AssociationDetailFilterViewModel filter);
    }
}
