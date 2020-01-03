namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Model.Home;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.ViewModel.Home;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    [NapSuperAuth]
    public class HomeController : BaseController
    {
        private readonly IAccountService accountService;

        public HomeController()
        {
            accountService = new AccountService();
        }

        public ActionResult Index()
        {
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

        [HttpPost]
        public ActionResult Search(FormCollection form)
        {
            string inputValue = form["txtInput"].Trim();

            if (string.IsNullOrEmpty(inputValue) || inputValue.Length <= 1 || inputValue.Length > 50)
            {
                return View(new SearchResultViewModel { SearchResults = null, InputValue = inputValue });
            }

            var result = accountService.SearchAccounts(inputValue);

            if (result.Count == 1 && result.Any(x => (x.Username != null && x.Username.Equals(inputValue, System.StringComparison.CurrentCultureIgnoreCase)) || (x.Username2 != null && x.Username2.Equals(inputValue, System.StringComparison.CurrentCultureIgnoreCase))))
            {
                return RedirectToAction("Index", "Account", new { Id = result.FirstOrDefault().CTSCustId });

            }

            return View(new SearchResultViewModel { SearchResults = result, InputValue = inputValue });
        }
    }
}