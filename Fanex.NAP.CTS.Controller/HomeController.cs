namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Model.Home;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.ViewModel.Home;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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

            var inputUsername = ValidateUserName(inputValue);

            var result = accountService.SearchAccounts(inputUsername);

            if (result.Count == 1 && result.Any(x => (
                        x.Username != null && x.Username.Equals(inputValue, System.StringComparison.CurrentCultureIgnoreCase)) 
                        || (x.Username2 != null && x.Username2.Equals(inputValue, System.StringComparison.CurrentCultureIgnoreCase))))
            {
                return RedirectToAction("Index", "Account", new { Id = result.FirstOrDefault().CTSCustId });

            }

            var tasks = new List<Task>();

            var exactUserName = new List<SearchResultModel>();
            tasks.Add(Task.Run(() => { exactUserName = result.Where(x => !string.IsNullOrWhiteSpace(x.Username) && x.Username.StartsWith(inputValue, System.StringComparison.CurrentCultureIgnoreCase)).ToList(); }));

            var exactUserName2 = new List<SearchResultModel>();
            tasks.Add(Task.Run(() => { exactUserName2 = result.Where(x => !string.IsNullOrWhiteSpace(x.Username2) && x.Username2.StartsWith(inputValue, System.StringComparison.CurrentCultureIgnoreCase)).ToList(); }));

            var likeUserName = new List<SearchResultModel>();
            tasks.Add(Task.Run(() => { likeUserName = result.Where(x => !string.IsNullOrWhiteSpace(x.Username) && x.Username.StartsWith(inputValue, System.StringComparison.CurrentCultureIgnoreCase)).ToList(); }));

            var likeUserName2 = new List<SearchResultModel>();
            tasks.Add(Task.Run(() => { likeUserName2 = result.Where(x => !string.IsNullOrWhiteSpace(x.Username2) && x.Username2.StartsWith(inputValue, System.StringComparison.CurrentCultureIgnoreCase)).ToList(); }));

            Task.WhenAll(tasks.ToArray()).Wait();

            var newResult = exactUserName
                .Union(exactUserName2)
                .Union(likeUserName)
                .Union(likeUserName2)
                .Union(result).ToList();

            return View(new SearchResultViewModel { SearchResults = newResult, InputValue = inputValue });
        }

        private string ValidateUserName(string inputUserName)
        {
            var words = inputUserName.Split(new char[] { '$', '@' });
            var result = new StringBuilder();

            if(words.Length > 0)
            {
                foreach(var word in words)
                {
                    if(!string.IsNullOrWhiteSpace(word))
                    {
                        if(word.Length > 1)
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
    }
}