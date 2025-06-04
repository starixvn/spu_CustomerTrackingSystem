using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fanex.NAP.CTS.Service.Implement.RobotReport;
using Fanex.NAP.CTS.Model.RobotReport;
using Fanex.NAP.CTS.Model.RobotReport.Entities;
using Fanex.NAP.CTS.Criteria.RobotReport;
using Fanex.Data.Repository;
using System.Collections.Generic;
using System.Linq;
using System;
using Moq;
using System.Reflection;
using Fanex.NAP.CTS.Core.Service;
using static Fanex.NAP.CTS.Resource.Consts.Consts;

namespace Fanex.NAP.CTS.Test.Services
{
    [TestClass]
    public class RobotReportServiceTest
    {
        private Mock<ICriteriaService> _mockRepository;
        private RobotReportService _robotReportService;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<ICriteriaService>();
            _robotReportService = new RobotReportService(_mockRepository.Object);
        }

        [TestMethod]
        public void GetRobotReportCounter_WithAllSites_SetsIsLicenseeTo2()
        {
            // Arrange
            var param = new RobotReportParamModel
            {
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                Sites = MMSites.All
            };

            var mockEntities = new List<RobotReportCounterEntity>
            {
                new RobotReportCounterEntity
                {
                    custid = 12345L,
                    RobotDate = new DateTime(2023, 1, 15)
                },
                new RobotReportCounterEntity
                {
                    custid = 12346L,
                    RobotDate = new DateTime(2023, 1, 20)
                }
            };

            ICriteria actualCriteria = null;
            _mockRepository.Setup(x => x.Fetch<RobotReportCounterEntity>(It.IsAny<ICriteria>()))
                          .Callback<ICriteria>(criteria => actualCriteria = criteria)
                          .Returns(mockEntities);

            // Act
            var result = InvokeGetRobotReportCounter(param);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(actualCriteria);
            Assert.IsInstanceOfType(actualCriteria, typeof(GetRobotReportCounterCriteria));

            var typedCriteria = actualCriteria as GetRobotReportCounterCriteria;
            Assert.AreEqual(param.FromDate, typedCriteria.FromDate);
            Assert.AreEqual(param.ToDate, typedCriteria.ToDate);
            Assert.AreEqual(2, typedCriteria.IsLicensee); // MMSites.All = 2

            var resultList = result.ToList();
            Assert.AreEqual(2, resultList.Count);

            // Verify first result
            var firstResult = resultList[0];
            Assert.AreEqual(12345L, firstResult.custid);
            Assert.AreEqual(new DateTime(2023, 1, 15), firstResult.RobotDate);

            // Verify second result
            var secondResult = resultList[1];
            Assert.AreEqual(12346L, secondResult.custid);
            Assert.AreEqual(new DateTime(2023, 1, 20), secondResult.RobotDate);

            _mockRepository.Verify(x => x.Fetch<RobotReportCounterEntity>(It.IsAny<GetRobotReportCounterCriteria>()), Times.Once);
        }

        [TestMethod]
        public void GetRobotReportCounter_WithLicenseeSites_SetsIsLicenseeTo1()
        {
            // Arrange
            var param = new RobotReportParamModel
            {
                FromDate = new DateTime(2023, 2, 1),
                ToDate = new DateTime(2023, 2, 28),
                Sites = MMSites.Licensee
            };

            var mockEntities = new List<RobotReportCounterEntity>
            {
                new RobotReportCounterEntity
                {
                    custid = 54321L,
                    RobotDate = new DateTime(2023, 2, 10)
                }
            };

            ICriteria actualCriteria = null;
            _mockRepository.Setup(x => x.Fetch<RobotReportCounterEntity>(It.IsAny<ICriteria>()))
                          .Callback<ICriteria>(criteria => actualCriteria = criteria)
                          .Returns(mockEntities);

            // Act
            var result = InvokeGetRobotReportCounter(param);

            // Assert
            var typedCriteria = actualCriteria as GetRobotReportCounterCriteria;
            Assert.AreEqual(1, typedCriteria.IsLicensee); // MMSites.Licensee = 1

            var resultList = result.ToList();
            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual(54321L, resultList[0].custid);
        }

        [TestMethod]
        public void GetRobotReportCounter_WithCreditSites_SetsIsLicenseeTo0()
        {
            // Arrange
            var param = new RobotReportParamModel
            {
                FromDate = new DateTime(2023, 3, 1),
                ToDate = new DateTime(2023, 3, 31),
                Sites = MMSites.Credit
            };

            var mockEntities = new List<RobotReportCounterEntity>
            {
                new RobotReportCounterEntity
                {
                    custid = 98765L,
                    RobotDate = new DateTime(2023, 3, 15)
                }
            };

            ICriteria actualCriteria = null;
            _mockRepository.Setup(x => x.Fetch<RobotReportCounterEntity>(It.IsAny<ICriteria>()))
                          .Callback<ICriteria>(criteria => actualCriteria = criteria)
                          .Returns(mockEntities);

            // Act
            var result = InvokeGetRobotReportCounter(param);

            // Assert
            var typedCriteria = actualCriteria as GetRobotReportCounterCriteria;
            Assert.AreEqual(0, typedCriteria.IsLicensee); // MMSites.Credit = 0

            var resultList = result.ToList();
            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual(98765L, resultList[0].custid);
        }

        [TestMethod]
        public void GetRobotReportCounter_WithMonthlyDate_UsesCorrectDateRange()
        {
            // Arrange
            var param = new RobotReportParamModel
            {
                FromDate = new DateTime(2023, 5, 15), // Will be adjusted to 2023-05-01
                ToDate = new DateTime(2023, 5, 20),   // Will be adjusted to 2023-06-01 00:00:00 - 1 second
                Sites = MMSites.All,
                IsMonthly = true
            };

            ICriteria actualCriteria = null;
            _mockRepository.Setup(x => x.Fetch<RobotReportCounterEntity>(It.IsAny<ICriteria>()))
                          .Callback<ICriteria>(criteria => actualCriteria = criteria)
                          .Returns(new List<RobotReportCounterEntity>());

            // Act
            var result = InvokeGetRobotReportCounter(param);

            // Assert
            var typedCriteria = actualCriteria as GetRobotReportCounterCriteria;
            Assert.AreEqual(new DateTime(2023, 5, 1), typedCriteria.FromDate); // First day of month
            Assert.AreEqual(new DateTime(2023, 6, 1).AddSeconds(-1), typedCriteria.ToDate); // Last second of month
        }

        [TestMethod]
        public void GetRobotReportCounter_WithDailyDate_UsesCorrectDateRange()
        {
            // Arrange
            var param = new RobotReportParamModel
            {
                FromDate = new DateTime(2023, 4, 10, 10, 30, 0),
                ToDate = new DateTime(2023, 4, 15, 15, 45, 0), // Will be adjusted to 2023-04-16 00:00:00 - 1 second
                Sites = MMSites.Licensee,
                IsDaily = true
            };

            ICriteria actualCriteria = null;
            _mockRepository.Setup(x => x.Fetch<RobotReportCounterEntity>(It.IsAny<ICriteria>()))
                          .Callback<ICriteria>(criteria => actualCriteria = criteria)
                          .Returns(new List<RobotReportCounterEntity>());

            // Act
            var result = InvokeGetRobotReportCounter(param);

            // Assert
            var typedCriteria = actualCriteria as GetRobotReportCounterCriteria;
            Assert.AreEqual(new DateTime(2023, 4, 10, 10, 30, 0), typedCriteria.FromDate); // Original FromDate
            Assert.AreEqual(new DateTime(2023, 4, 16).AddSeconds(-1), typedCriteria.ToDate); // End of ToDate day + 1 day - 1 second
        }

        [TestMethod]
        public void GetRobotReportCounter_WithNullRepositoryResult_ReturnsEmptyEnumerable()
        {
            // Arrange
            var param = new RobotReportParamModel
            {
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                Sites = MMSites.All
            };

            _mockRepository.Setup(x => x.Fetch<RobotReportCounterEntity>(It.IsAny<ICriteria>()))
                          .Returns((IEnumerable<RobotReportCounterEntity>)null);

            // Act
            var result = InvokeGetRobotReportCounter(param);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetRobotReportCounter_WithEmptyRepositoryResult_ReturnsEmptyEnumerable()
        {
            // Arrange
            var param = new RobotReportParamModel
            {
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                Sites = MMSites.Credit
            };

            _mockRepository.Setup(x => x.Fetch<RobotReportCounterEntity>(It.IsAny<ICriteria>()))
                          .Returns(new List<RobotReportCounterEntity>());

            // Act
            var result = InvokeGetRobotReportCounter(param);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetRobotReportCounter_EntityToModelMapping_PreservesAllProperties()
        {
            // Arrange
            var param = new RobotReportParamModel
            {
                FromDate = new DateTime(2023, 6, 1),
                ToDate = new DateTime(2023, 6, 30),
                Sites = MMSites.All
            };

            var testDate = new DateTime(2023, 6, 15, 14, 30, 45);
            var testCustId = 999888777L;

            var mockEntities = new List<RobotReportCounterEntity>
            {
                new RobotReportCounterEntity
                {
                    custid = testCustId,
                    RobotDate = testDate
                }
            };

            _mockRepository.Setup(x => x.Fetch<RobotReportCounterEntity>(It.IsAny<ICriteria>()))
                          .Returns(mockEntities);

            // Act
            var result = InvokeGetRobotReportCounter(param);

            // Assert
            var resultModel = result.First();
            Assert.AreEqual(testCustId, resultModel.custid);
            Assert.AreEqual(testDate, resultModel.RobotDate);
        }

        [TestMethod]
        public void GetRobotReportCounter_ParameterMapping_VerifiesCriteriaProperties()
        {
            // Arrange
            var specificFromDate = new DateTime(2023, 7, 10, 8, 15, 30);
            var specificToDate = new DateTime(2023, 7, 25, 18, 45, 0);

            var param = new RobotReportParamModel
            {
                FromDate = specificFromDate,
                ToDate = specificToDate,
                Sites = MMSites.Licensee
            };

            ICriteria actualCriteria = null;
            _mockRepository.Setup(x => x.Fetch<RobotReportCounterEntity>(It.IsAny<ICriteria>()))
                          .Callback<ICriteria>(criteria => actualCriteria = criteria)
                          .Returns(new List<RobotReportCounterEntity>());

            // Act
            InvokeGetRobotReportCounter(param);

            // Assert
            var typedCriteria = actualCriteria as GetRobotReportCounterCriteria;
            Assert.IsNotNull(typedCriteria);
            Assert.AreEqual(specificFromDate, typedCriteria.FromDate);
            Assert.AreEqual(new DateTime(2023, 7, 26).AddSeconds(-1), typedCriteria.ToDate); // Daily adjustment
            Assert.AreEqual(1, typedCriteria.IsLicensee);
        }

        [TestMethod]
        [ExpectedException(typeof(TargetInvocationException))]
        public void GetRobotReportCounter_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var param = new RobotReportParamModel
            {
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                Sites = MMSites.All
            };

            _mockRepository.Setup(x => x.Fetch<RobotReportCounterEntity>(It.IsAny<ICriteria>()))
                          .Throws(new Exception("Database connection error"));

            // Act
            InvokeGetRobotReportCounter(param);

            // Assert
            // ExpectedException should be thrown
        }

        [TestMethod]
        public void GetRobotReportCounter_MultipleEntities_ReturnsMappedResults()
        {
            // Arrange
            var param = new RobotReportParamModel
            {
                FromDate = new DateTime(2023, 8, 1),
                ToDate = new DateTime(2023, 8, 31),
                Sites = MMSites.Credit
            };

            var mockEntities = new List<RobotReportCounterEntity>
            {
                new RobotReportCounterEntity { custid = 1001L, RobotDate = new DateTime(2023, 8, 5) },
                new RobotReportCounterEntity { custid = 1002L, RobotDate = new DateTime(2023, 8, 10) },
                new RobotReportCounterEntity { custid = 1003L, RobotDate = new DateTime(2023, 8, 15) },
                new RobotReportCounterEntity { custid = 1004L, RobotDate = new DateTime(2023, 8, 20) },
                new RobotReportCounterEntity { custid = 1005L, RobotDate = new DateTime(2023, 8, 25) }
            };

            _mockRepository.Setup(x => x.Fetch<RobotReportCounterEntity>(It.IsAny<ICriteria>()))
                          .Returns(mockEntities);

            // Act
            var result = InvokeGetRobotReportCounter(param);

            // Assert
            var resultList = result.ToList();
            Assert.AreEqual(5, resultList.Count);

            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(1001L + i, resultList[i].custid);
                Assert.AreEqual(new DateTime(2023, 8, 5 + (i * 5)), resultList[i].RobotDate);
            }
        }

        [TestMethod]
        public void GetRobotReportCounter_WithMonthlyAndDailyFlags_MonthlyTakesPrecedence()
        {
            // Arrange
            var param = new RobotReportParamModel
            {
                FromDate = new DateTime(2023, 9, 15),
                ToDate = new DateTime(2023, 9, 20),
                Sites = MMSites.All,
                IsMonthly = true,
                IsDaily = true // Both flags set, Monthly should take precedence
            };

            ICriteria actualCriteria = null;
            _mockRepository.Setup(x => x.Fetch<RobotReportCounterEntity>(It.IsAny<ICriteria>()))
                          .Callback<ICriteria>(criteria => actualCriteria = criteria)
                          .Returns(new List<RobotReportCounterEntity>());

            // Act
            InvokeGetRobotReportCounter(param);

            // Assert
            var typedCriteria = actualCriteria as GetRobotReportCounterCriteria;
            Assert.AreEqual(new DateTime(2023, 9, 1), typedCriteria.FromDate); // Monthly adjustment
            Assert.AreEqual(new DateTime(2023, 10, 1).AddSeconds(-1), typedCriteria.ToDate); // Monthly adjustment
        }

        /// <summary>
        /// Helper method to invoke the private GetRobotReportCounter method using reflection
        /// </summary>
        private IEnumerable<RobotReportCounterModel> InvokeGetRobotReportCounter(RobotReportParamModel param)
        {
            var methodInfo = typeof(RobotReportService).GetMethod("GetRobotReportCounter", BindingFlags.NonPublic | BindingFlags.Instance);
            return (IEnumerable<RobotReportCounterModel>)methodInfo.Invoke(_robotReportService, new object[] { param });
        }
    }
} 