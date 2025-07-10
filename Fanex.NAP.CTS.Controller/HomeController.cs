namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Model.Home;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.ViewModel.Home;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Fanex.Logging;
    using System.Web;
    using Fanex.NAP.CTS.Core.Utilities;
    using System.IO;
    using System;

    [NapSuperAuth]
    public class HomeController : BaseController
    {
        private readonly IAccountService accountService;
        private readonly IUserService userService;

        public HomeController()
        {
            accountService = new AccountService();
            userService = new UserService();
        }

        [HttpGet]
        public void test()
        {
            LogService.Info($"CTS test site");
        }       

        public ActionResult Index()
        {
            ViewBag.UserId = CurrentSession.UserId;
            ViewBag.OnUserId = CurrentSession.OnUserId;
            ViewBag.IsAssociationPermission = userService.IsHavingPermission(CurrentSession.UserId, 
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.AssociatedAccountMonitor,
                            NAP.Core.Business.Constant.Policy.Read);

            ViewBag.IsProbationPermission = userService.IsHavingPermission(CurrentSession.UserId,
                       NapSettings.Settings.SiteId,
                       FSFunctionKeys.ProbationManagement,
                       NAP.Core.Business.Constant.Policy.Read);

            return View(CurrentSession.MenuItemList);
        }

        public override string BaseControllerName { get => Consts.MenuItemHome; }

        public ActionResult Menu(string activeMenu)
        {
            ViewBag.ActiveMenu = activeMenu ?? this.BaseControllerName;

            return PartialView("~/Views/Shared/_Menu.cshtml", CurrentSession.MenuItemList);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult Search()
        {
            return View(new SearchResultViewModel { SearchResults = new List<SearchResultModel>(), InputValue = string.Empty });
        }

        [HttpPost]
        public ActionResult Search(FormCollection form, HttpPostedFileBase uploadFile)
        {
            string inputValue = form["txtInput"]?.Trim();

            bool hasText = !string.IsNullOrEmpty(inputValue) && inputValue.Length > 1 && inputValue.Length <= 50;
            bool hasFile = uploadFile != null
                           && uploadFile.ContentLength > 0
                           && (uploadFile.FileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase)
                               || uploadFile.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase));

            if (hasFile && !hasText)
            {
                if (ConfigSettings.AIInsightsStoreFile)
                {
                    base.SaveUploadedFile(uploadFile);
                }
                else
                {
                    using (var ms = new MemoryStream())
                    {
                        uploadFile.InputStream.CopyTo(ms);
                        Session["UploadedFileName"] = uploadFile.FileName;
                        Session["UploadedFileToBase64String"] = Convert.ToBase64String(ms.ToArray());
                    }
                }

                TempData["HasFile"] = true;

                return RedirectToAction("Index", "AIInsights");
            }

            if (hasText)
            {
                var result = accountService.SearchAccounts(inputValue);

                if (result.Count == 1)
                {
                    return RedirectToAction("Index", "Account", new { Id = result.First().CTSCustId });
                }

                return View(new SearchResultViewModel { SearchResults = result, InputValue = inputValue });
            }

            return View(new SearchResultViewModel { SearchResults = null, InputValue = inputValue });
        }              

        private string ValidateUserName(string inputUserName)
        {
            var words = inputUserName.Split(new char[] { '$', '@', '-', '(', ')' });
            var result = new StringBuilder();

            if(words.Length == 1)
            {
                result.Append($"{words[0]}*");
            }
            else
            {
                foreach (var word in words)
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        if (word.Length <= 2)
                        {
                            result.Append($"{word} ");
                        }
                        else
                        {
                            result.Append($"+{word}* ");
                        }
                    }
                }
            }            

            return  "\'" + result.ToString() + "\'";
        }

        public ActionResult UpdateMenuSettingForLoginPartialPage()
        {
            ViewBag.IsAssociationPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.AssociatedAccountMonitor,
                            NAP.Core.Business.Constant.Policy.Read);

            ViewBag.IsProbationPermission = userService.IsHavingPermission(CurrentSession.UserId,
                       NapSettings.Settings.SiteId,
                       FSFunctionKeys.ProbationManagement,
                       NAP.Core.Business.Constant.Policy.Read);

            return PartialView("~/Views/Shared/_LoginPartial.cshtml");
        }

        public ActionResult UpdateMenuSettingForHomePartialPage()
        {
            ViewBag.IsAssociationPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.AssociatedAccountMonitor,
                            NAP.Core.Business.Constant.Policy.Read);

            ViewBag.IsProbationPermission = userService.IsHavingPermission(CurrentSession.UserId,
                       NapSettings.Settings.SiteId,
                       FSFunctionKeys.ProbationManagement,
                       NAP.Core.Business.Constant.Policy.Read);

            return PartialView("~/Views/Shared/_HomeLoginPartial.cshtml");
        }
    }
}