using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fanex.NAP.CTS.Service.Implement;
using Fanex.NAP.CTS.Model.DangerMonitor;
using Fanex.NAP.CTS.Model.DangerMonitor.Entities;
using Fanex.NAP.CTS.Criteria.DangerMonitor;
using Fanex.NAP.CTS.Core.Service;
using System.Collections.Generic;
using System.Linq;
using System;
using Moq;
using System.Reflection;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Test.Services
{
    [TestClass]
    public class DangerMonitorServiceTest
    {
        private Mock<ICriteriaService> _mockRepository;
        private DangerMonitorService _dangerMonitorService;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<ICriteriaService>();
            _dangerMonitorService = new DangerMonitorService(string.Empty, string.Empty, _mockRepository.Object);
        }

        [TestMethod]
        public void GetDangerMonitorInfoWithNoAssociation_WithValidFilter_ReturnsCorrectResults()
        {
            // Arrange
            var filter = new DangerMonitorSearchModel
            {
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                SportList = new List<string> { "1_null", "2_LeagueGroup" },
                Event = "Test Event",
                BetTypes = "1,2,3",
                MatchSection = "1",
                Market = "Market1,Market2",
                QueryDanger = "Danger1,Danger2",
                UserNameList = "User1,User2",
                LeagueGroupList = "LG1,LG2",
                TotalAmount = 1000,
                UserAmount = 500
            };

            var entities = new List<DangerMMTicketInfoEntity>
            {
                new DangerMMTicketInfoEntity
                {
                    MatchSection = 1,
                    EventDate = new DateTime(2023, 1, 15),
                    MatchId = 12345,
                    LiveIndicator = true,
                    Bettype = 1,
                    BetID = 101,
                    BetChoiceType = 1,
                    LeagueId = 201,
                    LeagueName = "Test League",
                    SportType = 1,
                    LeagueGroupID = 301,
                    GlobalShowTime = new DateTime(2023, 1, 15, 15, 30, 0),
                    Homeid = 401,
                    HomeName = "Home Team",
                    AwayId = 402,
                    AwayName = "Away Team",
                    EventStatus = "Live",
                    LiveHomeScore = 1,
                    LiveAwayScore = 0,
                    TotalAmountHome = 5000,
                    TotalAmountAway = 3000,
                    TotalTicketHome = 50,
                    TotalTicketAway = 30
                },
                new DangerMMTicketInfoEntity
                {
                    MatchSection = 2,
                    EventDate = new DateTime(2023, 1, 20),
                    MatchId = 12346,
                    LiveIndicator = false,
                    Bettype = 2,
                    BetID = 102,
                    BetChoiceType = 2,
                    LeagueId = 202,
                    LeagueName = "Test League 2",
                    SportType = 2,
                    LeagueGroupID = 302,
                    GlobalShowTime = new DateTime(2023, 1, 20, 18, 0, 0),
                    Homeid = 403,
                    HomeName = "Home Team 2",
                    AwayId = 404,
                    AwayName = "Away Team 2",
                    EventStatus = "Finished",
                    LiveHomeScore = 2,
                    LiveAwayScore = 1,
                    TotalAmountHome = 7000,
                    TotalAmountAway = 4000,
                    TotalTicketHome = 70,
                    TotalTicketAway = 40
                }
            };

            _mockRepository.Setup(x => x.Fetch<DangerMMTicketInfoEntity>(It.IsAny<DangerMonitorSearchMatchInfoCriteria>()))
                .Returns(entities);

            // Act
            var result = InvokeGetDangerMonitorInfoWithNoAssociation(filter);

            // Assert
            Assert.IsNotNull(result);
            var resultList = result.ToList();
            Assert.AreEqual(2, resultList.Count);

            var firstItem = resultList[0];
            Assert.AreEqual(1, firstItem.MatchSection);
            Assert.AreEqual(new DateTime(2023, 1, 15), firstItem.EventDate);
            Assert.AreEqual(12345, firstItem.MatchId);
            Assert.AreEqual(1, firstItem.LiveIndicator); // true converts to 1
            Assert.AreEqual(1, firstItem.Bettype);
            Assert.AreEqual(101, firstItem.BetID);

            var secondItem = resultList[1];
            Assert.AreEqual(0, secondItem.LiveIndicator); // false converts to 0
            Assert.AreEqual(12346, secondItem.MatchId);

            _mockRepository.Verify(x => x.Fetch<DangerMMTicketInfoEntity>(It.IsAny<DangerMonitorSearchMatchInfoCriteria>()), Times.Once);
        }

        [TestMethod]
        public void GetDangerMonitorInfoWithNoAssociation_WithEmptyResults_ReturnsEmptyList()
        {
            // Arrange
            var filter = new DangerMonitorSearchModel
            {
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31)
            };

            var entities = new List<DangerMMTicketInfoEntity>();

            _mockRepository.Setup(x => x.Fetch<DangerMMTicketInfoEntity>(It.IsAny<DangerMonitorSearchMatchInfoCriteria>()))
                .Returns(entities);

            // Act
            var result = InvokeGetDangerMonitorInfoWithNoAssociation(filter);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            _mockRepository.Verify(x => x.Fetch<DangerMMTicketInfoEntity>(It.IsAny<DangerMonitorSearchMatchInfoCriteria>()), Times.Once);
        }

        [TestMethod]
        public void GetDangerMonitorInfoWithNoAssociation_WithNullResults_ReturnsEmptyList()
        {
            // Arrange
            var filter = new DangerMonitorSearchModel
            {
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31)
            };

            _mockRepository.Setup(x => x.Fetch<DangerMMTicketInfoEntity>(It.IsAny<DangerMonitorSearchMatchInfoCriteria>()))
                .Returns((IEnumerable<DangerMMTicketInfoEntity>)null);

            // Act
            var result = InvokeGetDangerMonitorInfoWithNoAssociation(filter);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetDangerMonitorInfoWithNoAssociation_PassesCorrectCriteria()
        {
            // Arrange
            var filter = new DangerMonitorSearchModel
            {
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                SportList = new List<string> { "1_null", "2_LeagueGroup" },
                Event = "Test Event",
                BetTypes = "1,2,3",
                MatchSection = "1",
                Market = "Market1,Market2",
                QueryDanger = "Danger1,Danger2",
                UserNameList = "User1,User2",
                LeagueGroupList = "LG1,LG2",
                TotalAmount = 1000,
                UserAmount = 500
            };

            ICriteria actualCriteria = null;

            _mockRepository.Setup(x => x.Fetch<DangerMMTicketInfoEntity>(It.IsAny<ICriteria>()))
                .Callback<ICriteria>(criteria => actualCriteria = criteria)
                .Returns(new List<DangerMMTicketInfoEntity>());

            // Act
            InvokeGetDangerMonitorInfoWithNoAssociation(filter);

            // Assert
            Assert.IsNotNull(actualCriteria);
            Assert.IsInstanceOfType(actualCriteria, typeof(DangerMonitorSearchMatchInfoCriteria));

            var typedCriteria = actualCriteria as DangerMonitorSearchMatchInfoCriteria;
            Assert.IsNotNull(typedCriteria);
            Assert.AreEqual(filter.FromDate, typedCriteria.FromDate);
            Assert.AreEqual(filter.ToDate, typedCriteria.ToDate);
            Assert.AreEqual("1", typedCriteria.SportList); // Should extract sport numbers from null items
            Assert.AreEqual("2_LeagueGroup", typedCriteria.SportSabaLgGroup); // Should get non-null items
            Assert.AreEqual(filter.Event, typedCriteria.LeagueEvent);
            Assert.AreEqual(filter.BetTypes, typedCriteria.BettypeList);
            Assert.AreEqual(filter.MatchSection, typedCriteria.MatchSection);
            Assert.AreEqual(filter.Market, typedCriteria.MarketList);
            Assert.AreEqual(filter.QueryDanger, typedCriteria.QueryDanger);
            Assert.AreEqual(filter.UserNameList, typedCriteria.UserNameList);
            Assert.AreEqual(filter.LeagueGroupList, typedCriteria.LeagueGroupList);
            Assert.AreEqual(filter.TotalAmount, typedCriteria.TotalAmount);
            Assert.AreEqual(filter.UserAmount, typedCriteria.UserAmount);
        }

        [TestMethod]
        public void GetDangerMonitorInfoWithNoAssociation_WithNullOptionalFields_HandlesGracefully()
        {
            // Arrange
            var filter = new DangerMonitorSearchModel
            {
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                SportList = null,
                Event = null,
                QueryDanger = null,
                UserNameList = null
            };

            ICriteria actualCriteria = null;

            _mockRepository.Setup(x => x.Fetch<DangerMMTicketInfoEntity>(It.IsAny<ICriteria>()))
                .Callback<ICriteria>(criteria => actualCriteria = criteria)
                .Returns(new List<DangerMMTicketInfoEntity>());

            // Act
            InvokeGetDangerMonitorInfoWithNoAssociation(filter);

            // Assert
            var typedCriteria = actualCriteria as DangerMonitorSearchMatchInfoCriteria;
            Assert.IsNotNull(typedCriteria);
            Assert.AreEqual(string.Empty, typedCriteria.LeagueEvent);
            Assert.AreEqual(string.Empty, typedCriteria.QueryDanger);
            Assert.AreEqual(string.Empty, typedCriteria.UserNameList);
        }

        [TestMethod]
        public void GetDangerMonitorInfoWithNoAssociation_SportListProcessing_WorksCorrectly()
        {
            // Arrange
            var filter = new DangerMonitorSearchModel
            {
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                SportList = new List<string> { "1_null", "3_null", "2_LeagueGroup", "4_AnotherGroup" }
            };

            ICriteria actualCriteria = null;

            _mockRepository.Setup(x => x.Fetch<DangerMMTicketInfoEntity>(It.IsAny<ICriteria>()))
                .Callback<ICriteria>(criteria => actualCriteria = criteria)
                .Returns(new List<DangerMMTicketInfoEntity>());

            // Act
            InvokeGetDangerMonitorInfoWithNoAssociation(filter);

            // Assert
            var typedCriteria = actualCriteria as DangerMonitorSearchMatchInfoCriteria;
            Assert.IsNotNull(typedCriteria);
            Assert.AreEqual("1,3", typedCriteria.SportList); // Should get sports with "null"
            Assert.AreEqual("2_LeagueGroup,4_AnotherGroup", typedCriteria.SportSabaLgGroup); // Should get sports without "null"
        }

        [TestMethod]
        [ExpectedException(typeof(TargetInvocationException))]
        public void GetDangerMonitorInfoWithNoAssociation_WhenRepositoryThrows_PropagatesException()
        {
            // Arrange
            var filter = new DangerMonitorSearchModel
            {
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31)
            };

            _mockRepository.Setup(x => x.Fetch<DangerMMTicketInfoEntity>(It.IsAny<DangerMonitorSearchMatchInfoCriteria>()))
                .Throws(new Exception("Database error"));

            // Act
            InvokeGetDangerMonitorInfoWithNoAssociation(filter);

            // Assert
            // Exception should be thrown
        }

        // Helper method to invoke private method using reflection
        private IEnumerable<DangerMMTicketInfoModel> InvokeGetDangerMonitorInfoWithNoAssociation(DangerMonitorSearchModel filter)
        {
            var method = typeof(DangerMonitorService).GetMethod("GetDangerMonitorInfoWithNoAssociation",
                BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsNotNull(method, "GetDangerMonitorInfoWithNoAssociation method not found");

            return (IEnumerable<DangerMMTicketInfoModel>)method.Invoke(_dangerMonitorService, new object[] { filter });
        }
    }
}