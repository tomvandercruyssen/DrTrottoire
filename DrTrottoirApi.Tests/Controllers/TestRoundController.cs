using DrTrottoirApi.Controllers;
using DrTrottoirApi.Entities;
using DrTrottoirApi.Models;
using DrTrottoirApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Tests.Controllers
{
    [TestClass]
    public class TestRoundController
    {
        private readonly Guid _roundId1 = Guid.NewGuid();
        private readonly Guid _roundId2 = Guid.NewGuid();
        private readonly Guid _roundId3 = Guid.NewGuid();
        private readonly Guid _companyId1 = Guid.NewGuid();
        private readonly Guid _companyId2 = Guid.NewGuid();
        private readonly Guid _companyId3 = Guid.NewGuid();
        private readonly Guid _garbageCollectionId1 = Guid.NewGuid();
        private readonly Guid _garbageCollectionId2 = Guid.NewGuid();
        private readonly DateTime _collectionTime = new(2023, 6, 5, 9, 30, 0);

        [TestMethod]
        public async Task GetAllRounds_ShouldReturnListOfRounds()
        {
            var mockRepository = new Mock<IRoundRepository>();
            var controller = new RoundsController(mockRepository.Object);

            // Arrange
            var roundResponse = new List<BaseRoundResponse> { new BaseRoundResponse { Id = Guid.NewGuid() } };
            mockRepository.Setup(r => r.GetAllRounds()).ReturnsAsync(roundResponse);

            // Act
            var result = await controller.Get();
            var actionResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(actionResult.Value);
            Assert.AreEqual(200, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task GetRoundById_ShouldReturnRound()
        {
            var mockRepository = new Mock<IRoundRepository>();
            var controller = new RoundsController(mockRepository.Object);

            // Arrange
            var roundId = Guid.NewGuid();
            var roundResponse = new List<BaseRoundResponse> { new BaseRoundResponse { Id = roundId } };
            mockRepository.Setup(r => r.GetAllRounds()).ReturnsAsync(roundResponse);

            // Act
            var result = await controller.Get();
            var actionResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task GetRoundById_InvalidId_ShouldReturnBadRequest()
        {
            // Arrange
            var mockContext = new Mock<DrTrottoirDbContext>();
            var roundRepository = new RoundRepository(mockContext.Object);
            var roundController = new RoundsController(roundRepository);

            // Act
            var result = await roundController.Get(Guid.Empty);
            var actionResult = result.Result as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(400, actionResult.StatusCode);
        }


        [TestMethod]
        public async Task GetProgressOfRound_ShouldReturnTheCorrectProgress()
        {
            var contextMock = new Mock<DrTrottoirDbContext>();

            contextMock.Setup(c => c.Rounds).ReturnsDbSet(GetFakeRounds());
            contextMock.Setup(c => c.Tasks).ReturnsDbSet(GetFakeTasks());
            
            var repository = new RoundRepository(contextMock.Object);
            var controller = new RoundsController(repository);

            var result1 = await controller.GetRoundProgress(_roundId1);
            var actionResult1 = result1 as OkObjectResult;

            var result2 = await controller.GetRoundProgress(_roundId2);
            var actionResult2 = result2 as OkObjectResult;

            var result3 = await controller.GetRoundProgress(_roundId3);
            var actionResult3 = result3 as OkObjectResult;

            Assert.IsNotNull(actionResult1);
            Assert.IsNotNull(actionResult2);
            Assert.IsNotNull(actionResult3);

            Assert.AreEqual(100, actionResult1.Value);
            Assert.AreEqual(50, actionResult2.Value);
            Assert.AreEqual(33, actionResult3.Value);
        }

        [TestMethod]
        public async Task PostRound_ShouldReturnOkStatus()
        {
            var mockRepository = new Mock<IRoundRepository>();
            var controller = new RoundsController(mockRepository.Object);
            // Arrange
            var round = new Round();
            var user = new User()
            {
                UserName = "Test1",
                FirstName = "Test2",
                LastName = "Test3",
                Email = "Test4",
                PictureUrl = "Test",
                TelephoneNumber = "Test",
                WorkAreaId = Guid.NewGuid()
            };

            round.User = user;

            var request = new CreateRoundRequest()
            {
                EndTime = round.EndTime,
                StartTime = round.StartTime,
                UserId = round.UserId,
                Name = round.Name
            };
            var result = await controller.Post(request);
            
            // Assert
            Assert.IsNotNull(result);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task UpdateRound_ShouldReturnOkStatus()
        {
            var mockRepository = new Mock<IRoundRepository>();
            var controller = new RoundsController(mockRepository.Object);
            // Arrange
            var round = new Round();
            Guid id = Guid.NewGuid();
            round.Id = id;
            var request = new CreateRoundRequest()
            {
                EndTime = round.EndTime,
                StartTime = round.StartTime,
                UserId = round.UserId,
                Name = round.Name
            };
            var result = await controller.Post(request);
            
            round.Name = "changed";
            
            // Act
            var result2 = await controller.Put(round.Id, round);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsInstanceOfType(result2, typeof(OkResult));
        }


        [TestMethod]
        public async Task UpdateRound_ShouldReturnBadRequest()
        {
            // Arrange
            var mockContext = new Mock<DrTrottoirDbContext>();
            var roundRepository = new RoundRepository(mockContext.Object);
            var roundController = new RoundsController(roundRepository);
            var round = new Round();
            var request = new CreateRoundRequest()
            {
                EndTime = round.EndTime,
                StartTime = round.StartTime,
                UserId = round.UserId,
                Name = round.Name
            };

            // Act
            var result = await roundController.Put(Guid.Empty, round);
            var actionResult = result as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(400, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteRound_ShouldReturnOkStatus()
        {
            var mockRepository = new Mock<IRoundRepository>();
            var controller = new RoundsController(mockRepository.Object);

            var request = new CreateRoundRequest();
            var result = await controller.Post(request);
            var roundId = ((OkObjectResult)result).Value as Guid?;
            // Act
            var result2 = await controller.Delete((Guid)roundId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsInstanceOfType(result2, typeof(OkResult));
        }

        [TestMethod]
        public async Task DeleteRound_InvalidId_ShouldReturnBadRequest()
        {
            // Arrange
            var mockContext = new Mock<DrTrottoirDbContext>();
            var roundRepository = new RoundRepository(mockContext.Object);
            var roundController = new RoundsController(roundRepository);

            // Act
            var result = await roundController.Delete(Guid.Empty);
            var actionResult = result as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(400, actionResult.StatusCode);
        }
        [TestMethod]
        public async Task GetTasksForRound_ShouldReturnBadRequest()
        {
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(x => x.Rounds).ReturnsDbSet(new List<Round>());

            var mockRepository = new RoundRepository(mockContext.Object);
            var controller = new RoundsController(mockRepository);

            var result = await controller.GetTasksForRound(Guid.NewGuid());

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
        [TestMethod]
        public async Task GetTasksForRound_ShouldReturnTheCorrectTasks()
        {
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Rounds).ReturnsDbSet(GetFakeRounds());
            mockContext.Setup(c => c.Tasks).ReturnsDbSet(GetFakeTasks());
            mockContext.Setup(c => c.Companies).ReturnsDbSet(GetFakeCompanies());
            mockContext.Setup(c => c.GarbageTypes).ReturnsDbSet(GetFakeGarbageTypes());
            mockContext.Setup(c => c.GarbageCollections).ReturnsDbSet(GetFakeGarbageCollections());
            mockContext.Setup(c => c.CompanyGarbageCollections).ReturnsDbSet(GetFakeCompanyGarbageCollections());
            mockContext.Setup(c => c.GarbageCollectionGarbageTypes).ReturnsDbSet(GetFakeGarbageCollectionGarbageType());

            var roundRepo = new RoundRepository(mockContext.Object);
            var roundController = new RoundsController(roundRepo);

            var result = await roundController.GetTasksForRound(_roundId1);
            var value = (result as OkObjectResult)?.Value as List<GetTasksForRoundsResponse>;
            
            Assert.IsNotNull(value);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            Assert.AreEqual(2, value.Count);
            Assert.AreEqual(1, value[0].GarbageTypesInside.Count);
            Assert.AreEqual(1, value[0].GarbageTypesOutside.Count);
            Assert.AreEqual("REST", value[0].GarbageTypesInside[0].Name);
            Assert.AreEqual("GLAS", value[1].GarbageTypesOutside[0].Name);
        }

        [TestMethod]
        public async Task StartRound_ShouldReturnOkStatus()
        {
            // Arrange
            var mockRepository = new Mock<IRoundRepository>();
            var controller = new RoundsController(mockRepository.Object);

            var roundId = Guid.NewGuid();

            // Act
            var result = await controller.StartRound(roundId);
            var actionResult = result as OkResult;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(200, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task StartRound_ShouldReturnBadRequest()
        {
            // Arrange
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Rounds).ReturnsDbSet(GetFakeRounds());

            var roundRepo = new RoundRepository(mockContext.Object);
            var roundController = new RoundsController(roundRepo);

            var roundId = Guid.NewGuid();

            // Act
            var result = await roundController.StartRound(roundId);
            var actionResult = result as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(400, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task GetRemarksOfRound_ShouldReturnListOfRemarksAndCompanies()
        {
            // Arrange
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Rounds).ReturnsDbSet(GetFakeRounds());
            mockContext.Setup(c => c.Tasks).ReturnsDbSet(GetFakeTasks());
            mockContext.Setup(c => c.Companies).ReturnsDbSet(GetFakeCompanies());
            mockContext.Setup(c => c.GarbageTypes).ReturnsDbSet(GetFakeGarbageTypes());
            mockContext.Setup(c => c.GarbageCollections).ReturnsDbSet(GetFakeGarbageCollections());
            mockContext.Setup(c => c.CompanyGarbageCollections).ReturnsDbSet(GetFakeCompanyGarbageCollections());
            mockContext.Setup(c => c.GarbageCollectionGarbageTypes).ReturnsDbSet(GetFakeGarbageCollectionGarbageType());

            var roundRepo = new RoundRepository(mockContext.Object);
            var roundController = new RoundsController(roundRepo);

            var remarks = new List<GetRemarksOfRoundResponse> { };

            // Act
            var result = await roundController.GetRemarksOfRound(_roundId1);
            var actionResult = result as OkObjectResult;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(actionResult.Value);
            Assert.AreEqual(200, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task GetRemarksOfRound_ShouldReturnBadRequest()
        {
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Rounds).ReturnsDbSet(GetFakeRounds());

            var roundRepo = new RoundRepository(mockContext.Object);
            var roundController = new RoundsController(roundRepo);


            var roundId = Guid.Parse("c6c2244c-d8af-4b8f-8853-f235077f0765");

            // Act
            var result = await roundController.GetRemarksOfRound(roundId);
            var actionResult = result as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(400, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task GetByRound_ShouldReturnListOfCompanies()
        {
            var mockRepository = new Mock<IRoundRepository>();
            var controller = new RoundsController(mockRepository.Object);

            var roundId = Guid.NewGuid();
            var companies = new List<Company> {  };
            mockRepository.Setup(r => r.GetCompaniesByRound(roundId)).ReturnsAsync(companies);

            // Act
            var result = await controller.GetByRound(roundId);
            var actionResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(actionResult.Value);
            Assert.AreEqual(200, actionResult.StatusCode);

            var returnedCompanies = actionResult.Value as List<Company>;
            Assert.AreEqual(companies.Count, returnedCompanies.Count);
        }


        private List<Company> GetFakeCompanies()
        {
            return new List<Company>()
            {
                new() { Address = "BricoAd", Id = _companyId1, Name = "Brico" },
                new() { Address = "CarrefourAd", Id = _companyId2, Name = "Carrefour" },
                new() { Address = "AldiAd", Id = _companyId3, Name = "Aldi" }
            };
        }
        private List<GarbageType> GetFakeGarbageTypes()
        {
            var garbageTypes = new[]
            {
                new GarbageType() { Id = 1, Name = "REST" },
                new GarbageType() { Id = 2, Name = "GLAS" },
                new GarbageType() { Id = 3, Name = "PAPIER" },
                new GarbageType() { Id = 4, Name = "PMD" },
                new GarbageType() { Id = 5, Name = "GFT" },
            };

            return garbageTypes.ToList();
        }
        private List<GarbageCollection> GetFakeGarbageCollections()
        {
            return new List<GarbageCollection>()
            {
                new() { Id = _garbageCollectionId1, CollectionTime = _collectionTime, HasToBeBroughtOutside = true}, 
                new() { Id = _garbageCollectionId2, CollectionTime = _collectionTime.AddHours(4), HasToBeBroughtOutside = false}
            };
        }
        private List<CompanyGarbageCollection> GetFakeCompanyGarbageCollections()
        {
            var fakeCompanies = GetFakeCompanies();
            var fakeGarbageCollections = GetFakeGarbageCollections();
            return new List<CompanyGarbageCollection>() 
            { 
                new() { Company = fakeCompanies[0], CompanyId = _companyId1, GarbageCollectionId = _garbageCollectionId1, GarbageCollection = fakeGarbageCollections[0] }, 
                new() { Company = fakeCompanies[0], CompanyId = _companyId1, GarbageCollection = fakeGarbageCollections[1], GarbageCollectionId = _garbageCollectionId2 },
                new() { Company = fakeCompanies[1], CompanyId = _companyId2, GarbageCollectionId = _garbageCollectionId1, GarbageCollection = fakeGarbageCollections[0] },
                new() { Company = fakeCompanies[1], CompanyId = _companyId2, GarbageCollection = fakeGarbageCollections[1], GarbageCollectionId = _garbageCollectionId2 }
            };
        }
        private List<GarbageCollectionGarbageType> GetFakeGarbageCollectionGarbageType()
        {
            return new List<GarbageCollectionGarbageType>() 
            { 
                new() { GarbageTypeId = 1, GarbageCollectionId = _garbageCollectionId1, GarbageCollection = GetFakeGarbageCollections()[0], GarbageType = GetFakeGarbageTypes()[0]}, 
                new() { GarbageTypeId = 2, GarbageCollectionId = _garbageCollectionId2, GarbageCollection = GetFakeGarbageCollections()[1], GarbageType = GetFakeGarbageTypes()[1]}
            };
        }
        private List<Entities.Task> GetFakeTasks()
        {
            return new List<Entities.Task>()
                { 
                    new() { RoundId = _roundId1, Status = TaskStatus.Finished, StartTime = _collectionTime, EndTime = _collectionTime.AddHours(1), Company = GetFakeCompanies()[0]},
                    new() { RoundId = _roundId1, Status = TaskStatus.Finished, StartTime = _collectionTime.AddHours(2), EndTime = _collectionTime.AddHours(3), Company = GetFakeCompanies()[1]},
                    new() { RoundId = _roundId2, Status = TaskStatus.Finished},
                    new() { RoundId = _roundId2, Status = TaskStatus.InProgress},
                    new() { RoundId = _roundId3, Status = TaskStatus.Finished},
                    new() { RoundId = _roundId3, Status = TaskStatus.NotStarted},
                    new() { RoundId = _roundId3, Status = TaskStatus.InProgress}
                };
        }
        private List<Round> GetFakeRounds()
        {
            return new List<Round>()
            {
                new() { Id = _roundId1, StartTime = _collectionTime, EndTime = _collectionTime.AddHours(3)},
                new() { Id = _roundId2, StartTime = _collectionTime, EndTime = _collectionTime.AddHours(3)},
                new() { Id = _roundId3, StartTime = _collectionTime, EndTime = _collectionTime.AddHours(3)}
            };
        }
    }
}