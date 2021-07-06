using Fanex.NAP.Core.Web.Attribute;
using Fanex.NAP.CTS.Model.AssociationDetection;
using Fanex.NAP.CTS.Model.Common;
using Fanex.NAP.CTS.Model.CustomerClassify;
using Fanex.NAP.CTS.Service;
using Fanex.NAP.CTS.Service.Implement;
using Fanex.NAP.CTS.Service.Interface;
using Fanex.NAP.CTS.ViewModel.AssociationDetection;
using Fanex.NAP.CTS.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Fanex.NAP.CTS.Controller
{
    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.AssociationDetection, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class AssociationDetectionController : BaseController
    {
        private readonly ICommonService _commonService;
        private readonly IAssociationDetectionService _associationDetectionService;
        private readonly ICustomerClassifyService _customerClassifyService;

        public AssociationDetectionController()
        {
            _commonService = new CommonService();

            _associationDetectionService = new AssociationDetectionService();
            _customerClassifyService = new CustomerClassifyService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ValidateUsernames(string userNames)
        {
            var viewModel = ValidatedAccount(userNames);

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAssociationDetection(string userNames)
        {
            var isException = false;

            if (!string.IsNullOrWhiteSpace(userNames))
            {
                var userNameList = userNames.Split(',');

                var newUserNames = string.Join(",", userNameList.Distinct());

                var validatedList = _commonService.ValidateUsernames(newUserNames, out isException);

                var ctsCustIds = string.Join(",", validatedList.Select(x => x.CTSCustID));

                var groupLevel2ByUsername = _associationDetectionService.GetAssociationDetection(ctsCustIds);

                var custIds = string.Join(",", validatedList.Select(x => x.CustID));

                var latestCustomerClassification = _customerClassifyService.GetLatestClassification(custIds);

                var result = TransformLatestCustomerClassification(groupLevel2ByUsername, latestCustomerClassification);

                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(new List<GetAssociationDetectionModel>(), JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<GetAssociationDetectionViewModel> TransformLatestCustomerClassification(IEnumerable<GetAssociationDetectionModel> groupLevel2ByUsername, IEnumerable<LatestClassificationModel> latestCustomerClassification)
        {
            if (groupLevel2ByUsername != null && groupLevel2ByUsername.Any())
            {
                return groupLevel2ByUsername.Select(x =>
                {

                    var customerClass = x.CustSubId == 0 && x.CustomerCategory != null ? latestCustomerClassification.FirstOrDefault(a => a.CustId == x.CustId).CustomerClass : 0;

                    return new GetAssociationDetectionViewModel
                    {
                        GroupId = x.GroupId,
                        UserName = x.UserName,
                        CustomerCategory = x.CustomerCategory,
                        CategoryIds = x.CategoryIds,
                        Evidence = x.Evidence,
                        Status = x.Status,
                        CustomerClass = customerClass > 0 ? customerClass.ToString() : null,
                        IsProblemAccount = x.ParentIds != null && x.ParentIds.Split(',').Any(y => y == "50"),
                    };
                });
            }

            return new List<GetAssociationDetectionViewModel>();
        }

        private ValidatedAccountsViewModel ValidatedAccount(string userNames)
        {
            var isException = false;
            var viewModel = new ValidatedAccountsViewModel { IsException = false };

            if (!string.IsNullOrEmpty(userNames))
            {
                var userNameList = userNames.Split(',');
                var userNameToUpperList = userNameList.Select(x => x.ToUpper());

                var duplicateToUpperList = userNameToUpperList.GroupBy(x => x)
                       .Where(g => g.Count() > 1)
                       .Select(x => x.Last());

                var noDuplicateToUpperList = userNameToUpperList.Distinct();
                var noDuplicateToUpperUserName = string.Join(",", noDuplicateToUpperList);

                var validatedList = _commonService.ValidateUsernames(noDuplicateToUpperUserName, out isException);
                var validatedToUpperList = validatedList.Select(x => new ValidateUserNameModel
                {
                    CTSCustID = x.CTSCustID,
                    CustID = x.CustID,
                    UserName = x.UserName.ToUpper()
                });

                viewModel = GenerateValidatedViewModel(userNames, validatedToUpperList, isException);

                viewModel.UserNameDuplicatedList = userNameList.Where(x => duplicateToUpperList.Contains(x.ToUpper()));
                viewModel.UserNameValidList = validatedList.Select(x => x.UserName);
            }

            return viewModel;
        }

        private ValidatedAccountsViewModel GenerateValidatedViewModel(string userNames, IEnumerable<ValidateUserNameModel> validatedToUpperList, bool isException)
        {
            var userNameList = userNames.Split(',');
            var invalidList = Enumerable.Empty<string>();

            if (!isException)
            {
                var userNameToUpperValidatedList = validatedToUpperList.Select(x => x.UserName.ToUpper());

                invalidList = userNameList.Where(x => !userNameToUpperValidatedList.Contains(x.ToUpper()));
            }
            else
                invalidList = userNameList;

            return new ValidatedAccountsViewModel
            {
                IsException = isException,
                UserNameInvalidList = invalidList
            };
        }
    }
}
