namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountManagement;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.ProblemAccountManagement, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class ProblemAccountManagementController : BaseController
    {
        private readonly IProblemAccountManagementService _problemAccountManagementService;        

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public ProblemAccountManagementController()
        {
            _problemAccountManagementService = new ProblemAccountManagementService();
        }

        public ActionResult Index()
        {
            var sites = _problemAccountManagementService.GetAllSites();

            return View(sites);
        }

        [HttpPost]
        public ActionResult GetProblemAccount(ProblemAccountFilterViewModel filter)
        {
            var problemAccounts = Enumerable.Empty<ProblemAccountViewModel>();
            var paModel = _problemAccountManagementService.GetProblemAccounts(filter);
            
            if (paModel != null && paModel.Any())
            {
                var custIdList = string.Join(",", paModel.Select(x => x.CustID).Distinct());

                var paCommentSelModel = _problemAccountManagementService.GetProblemAccountCommentSel(custIdList);

                problemAccounts = paModel.OrderByDescending(x=>x.CreatedDate).Select(x => new ProblemAccountViewModel
                {
                    UserName = x.UserName,
                    Site = x.Site,
                    Currency = x.Currency,
                    Category = x.CategoryName,
                    Comment = paCommentSelModel.Any(y=>y.Custid == x.CustID) ? paCommentSelModel.FirstOrDefault(y=>y.Custid == x.CustID).Comment : string.Empty,
                    CreatedDateStr = x.CreatedDate.ToString(Consts.FormatDateToString_MMMddyyyy)
                });
            }

            return Json(new
            {
                data = problemAccounts
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetProblemAccountAllCategories()
        {
            var data = _problemAccountManagementService.GetProblemAccountCategories();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ValidateProblemAccountInput(ValidateProblemAccountInputViewModel model)
        {
            model.UserID = CurrentSession.UserId;

            var result = new ResultValidatedViewModel();

            var validatedModels = _problemAccountManagementService.ValidateProblemAccounts(model);

            if (validatedModels != null && validatedModels.Any())
            {
                result = GetResultValidated(validatedModels);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MarkProblemAccount(ValidateProblemAccountInputViewModel model)
        {
            model.UserID = CurrentSession.UserId;

            var result = new ResultMarkProblemAccountViewModel();

            var validatedModels = _problemAccountManagementService.ValidateProblemAccounts(model);

            if (validatedModels != null && validatedModels.Any())
            {
                var resultValidate = GetResultValidated(validatedModels);

                if (resultValidate.IsException == false 
                    && !resultValidate.ProblemAccountDuplicateList.Any() 
                    && !resultValidate.ProblemAccountInvalidList.Any())
                {
                   var paMarkResultList = _problemAccountManagementService.MarkNewProblemAccount(validatedModels);

                    if (paMarkResultList.Any())
                    {
                        result.ErrorCode = paMarkResultList.ToList()[0].ErrCode;
                        result.ErrorMessage = string.Join(",", paMarkResultList.Select(x => x.ErrMsg));
                        result.ResultValidated = resultValidate;
                    }
                }
                else
                {
                    result.ErrorCode = -1;
                    result.ResultValidated = resultValidate;
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private ResultValidatedViewModel GetResultValidated(IEnumerable<ProblemAccountValidatedModel> validatedModels)
        {
            var result = new ResultValidatedViewModel
            {
                IsException = false,
                ProblemAccountDuplicateList = validatedModels
                                                    .Where(x => x.ErrorMsg == Consts.ProblemAccountManagement_DuplicateUserName)
                                                    .Select(x => new ProblemAccountValidatedViewModel
                                                    {
                                                        UserName = x.UserName,
                                                        ErrMsg = x.ErrorMsg
                                                    }),
                ProblemAccountInvalidList = validatedModels
                                                    .Where(x => !string.IsNullOrEmpty(x.ErrorMsg) && x.ErrorMsg != Consts.ProblemAccountManagement_DuplicateUserName)
                                                    .Select(x => new ProblemAccountValidatedViewModel
                                                    {
                                                        UserName = x.UserName,
                                                        ErrMsg = x.ErrorMsg
                                                    })
            };

            return result;
        }

    }
}
