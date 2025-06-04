namespace Fanex.NAP.CTS.Test.Services
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Model.Common.Entities;
    using Fanex.NAP.CTS.Criteria.Common;
    using Fanex.NAP.CTS.Core.Service;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Moq;
    using Fanex.Data.Repository;

    [TestClass]
    public class CommonServiceTest
    {
        private Mock<ICriteriaService> _mockRepository;
        private CommonService _commonService;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<ICriteriaService>();
            _commonService = new CommonService(_mockRepository.Object);
        }

        [TestMethod]
        public void GetStaticLists_WithValidParameters_ReturnsCorrectResults()
        {
            // Arrange
            var listID = 16;
            var isStatusActive = true;
            var entities = new List<StaticListEntity>
            {
                new StaticListEntity
                {
                    ListID = 16,
                    ItemID = 1,
                    ListName = "Reason",
                    ListNameDisplay = "Reason Display",
                    ItemName = "Item1",
                    ItemNameDisplay = "Item 1 Display",
                    ItemValue = "Value1",
                    ItemDefaultValue = "Default1",
                    PriorityOrder = 1,
                    Status = true,
                    Description = "Test Description 1"
                },
                new StaticListEntity
                {
                    ListID = 16,
                    ItemID = 2,
                    ListName = "Reason",
                    ListNameDisplay = "Reason Display",
                    ItemName = "Item2",
                    ItemNameDisplay = "Item 2 Display",
                    ItemValue = "Value2",
                    ItemDefaultValue = "Default2",
                    PriorityOrder = 2,
                    Status = false,
                    Description = "Test Description 2"
                }
            };

            _mockRepository.Setup(x => x.Fetch<StaticListEntity>(It.Is<GetCommonStaticListCriteria>(
                c => c.ListID == listID && c.IsStatusActive == isStatusActive)))
                .Returns(entities);

            // Act
            var result = _commonService.GetStaticLists(listID, isStatusActive);

            // Assert
            Assert.IsNotNull(result);
            var resultList = result.ToList();
            Assert.AreEqual(2, resultList.Count);

            var firstItem = resultList[0];
            Assert.AreEqual(16, firstItem.ListID);
            Assert.AreEqual(1, firstItem.ItemID);
            Assert.AreEqual("Reason", firstItem.ListName);
            Assert.AreEqual("Reason Display", firstItem.ListNameDisplay);
            Assert.AreEqual("Item1", firstItem.ItemName);
            Assert.AreEqual("Item 1 Display", firstItem.ItemNameDisplay);
            Assert.AreEqual("Value1", firstItem.ItemValue);
            Assert.AreEqual("Default1", firstItem.ItemDefaultValue);
            Assert.AreEqual(1, firstItem.PriorityOrder);
            Assert.AreEqual(1, firstItem.Status); // true converts to 1
            Assert.AreEqual("Test Description 1", firstItem.Description);

            var secondItem = resultList[1];
            Assert.AreEqual(0, secondItem.Status); // false converts to 0

            _mockRepository.Verify(x => x.Fetch<StaticListEntity>(It.IsAny<GetCommonStaticListCriteria>()), Times.Once);
        }

        [TestMethod]
        public void GetStaticLists_WithEmptyResults_ReturnsEmptyList()
        {
            // Arrange
            var listID = 99;
            var isStatusActive = false;
            var entities = new List<StaticListEntity>();

            _mockRepository.Setup(x => x.Fetch<StaticListEntity>(It.Is<GetCommonStaticListCriteria>(
                c => c.ListID == listID && c.IsStatusActive == isStatusActive)))
                .Returns(entities);

            // Act
            var result = _commonService.GetStaticLists(listID, isStatusActive);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            _mockRepository.Verify(x => x.Fetch<StaticListEntity>(It.IsAny<GetCommonStaticListCriteria>()), Times.Once);
        }

        [TestMethod]
        public void GetStaticLists_WithNullResults_ReturnsEmptyList()
        {
            // Arrange
            var listID = 1;
            var isStatusActive = true;

            _mockRepository.Setup(x => x.Fetch<StaticListEntity>(It.IsAny<GetCommonStaticListCriteria>()))
                .Returns((IEnumerable<StaticListEntity>)null);

            // Act
            var result = _commonService.GetStaticLists(listID, isStatusActive);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            _mockRepository.Verify(x => x.Fetch<StaticListEntity>(It.IsAny<GetCommonStaticListCriteria>()), Times.Once);
        }

        [TestMethod]
        public void GetStaticLists_PassesCorrectCriteria()
        {
            // Arrange
            var listID = 17;
            var isStatusActive = false;
            ICriteria actualCriteria = null;

            _mockRepository.Setup(x => x.Fetch<StaticListEntity>(It.IsAny<ICriteria>()))
                .Callback<ICriteria>(criteria => actualCriteria = criteria)
                .Returns(new List<StaticListEntity>());

            // Act
            _commonService.GetStaticLists(listID, isStatusActive);

            // Assert
            Assert.IsNotNull(actualCriteria);
            Assert.IsInstanceOfType(actualCriteria, typeof(GetCommonStaticListCriteria));
            
            var typedCriteria = actualCriteria as GetCommonStaticListCriteria;
            Assert.IsNotNull(typedCriteria);
            Assert.AreEqual(listID, typedCriteria.ListID);
            Assert.AreEqual(isStatusActive, typedCriteria.IsStatusActive);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetStaticLists_WhenRepositoryThrows_PropagatesException()
        {
            // Arrange
            var listID = 1;
            var isStatusActive = true;

            _mockRepository.Setup(x => x.Fetch<StaticListEntity>(It.IsAny<GetCommonStaticListCriteria>()))
                .Throws(new Exception("Database error"));

            // Act
            _commonService.GetStaticLists(listID, isStatusActive);

            // Assert
            // Exception should be thrown
        }

        [TestMethod]
        public void GetStaticLists_StatusConversion_WorksCorrectly()
        {
            // Arrange
            var entities = new List<StaticListEntity>
            {
                new StaticListEntity { Status = true, ListID = 1, ItemID = 1 },
                new StaticListEntity { Status = false, ListID = 1, ItemID = 2 }
            };

            _mockRepository.Setup(x => x.Fetch<StaticListEntity>(It.IsAny<GetCommonStaticListCriteria>()))
                .Returns(entities);

            // Act
            var result = _commonService.GetStaticLists(1, true);

            // Assert
            var resultList = result.ToList();
            Assert.AreEqual(1, resultList[0].Status); // true -> 1
            Assert.AreEqual(0, resultList[1].Status); // false -> 0
        }
    }
}
