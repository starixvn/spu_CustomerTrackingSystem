using Fanex.NAP.CTS.Controller.Test.DbSetting;
using Fanex.NAP.CTS.Test.Fake;
using Fanex.NAP.CTS.ViewModel.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Xunit;

namespace Fanex.NAP.CTS.Controller.Test
{
    [Collection(nameof(DbSettingProviderCollection))]
    public class HomeControllerTest
    {
        private readonly HomeController _runner;

        public HomeControllerTest()
        {
            _runner = new HomeController();
        }

        //[Fact]
        //public void Search_Redirect()
        //{
        //    // Arrange
        //    FakeHttpContext.SetCurrentContext();
        //    FakeHttpContext.SetFakeSession();
        //    _runner.CurrentSession = FakeSession.CurrentSession;

        //    var username = "aaaWMSS";
        //    FormCollection collection = new FormCollection();
        //    collection.Add("txtInput", username);

        //    // Act
        //    var actualResult = _runner.Search(collection) as RedirectToRouteResult;

        //    // Assert
        //    Assert.Equal("Index", actualResult.RouteValues["action"]);
        //}

        //[Fact]
        //public void Search_ReturnViewModel()
        //{
        //    // Arrange
        //    FakeHttpContext.SetCurrentContext();
        //    FakeHttpContext.SetFakeSession();
        //    _runner.CurrentSession = FakeSession.CurrentSession;

        //    var username = "aaa";
        //    FormCollection collection = new FormCollection();
        //    collection.Add("txtInput", username);

        //    // Act
        //    var actualResult = _runner.Search(collection) as ViewResult;

        //    var viewModel = actualResult.ViewData.Model as SearchResultViewModel;

        //    // Assert
        //    Assert.Equal(username, viewModel.InputValue);
        //    Assert.True(viewModel.SearchResults.Any());
        //}

        //[Fact]
        //public void Search_ReturnNull()
        //{
        //    // Arrange
        //    FakeHttpContext.SetCurrentContext();
        //    FakeHttpContext.SetFakeSession();
        //    _runner.CurrentSession = FakeSession.CurrentSession;

        //    var username = "sfsdfs";
        //    FormCollection collection = new FormCollection();
        //    collection.Add("txtInput", username);

        //    // Act
        //    var actualResult = _runner.Search(collection) as ViewResult;

        //    var viewModel = actualResult.ViewData.Model as SearchResultViewModel;

        //    // Assert
        //    Assert.True(!viewModel.SearchResults.Any());
        //}
    }
}
