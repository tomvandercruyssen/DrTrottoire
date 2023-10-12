using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using DrTrottoirApi.Controllers;
using DrTrottoirApi.Entities;
using DrTrottoirApi.Models;
using DrTrottoirApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Tests.Controllers
{
    [TestClass]
    public class TestGarbageCollectionController
    {
        private readonly Guid _companyId = Guid.NewGuid();
        private readonly Guid _garbageCollectionId1 = Guid.NewGuid();
        private readonly Guid _garbageCollectionId2 = Guid.NewGuid();
        private readonly DateTime _collectionTime = new(2023, 6, 5, 9, 30, 0);

        [TestMethod]
        public async Task CreateGarbageCollection_ShouldReturnOkStatus()
        {
            var companies = GetFakeCompanies();
            var garbageTypes = GetFakeGarbageTypes();
            var testGarbageTypes = new List<string>()
                { garbageTypes[0].Name, garbageTypes[2].Name, garbageTypes[3].Name }; // REST, PAPIER, PMD

            var mockGarbageCollection = new Mock<Microsoft.EntityFrameworkCore.DbSet<GarbageCollection>>();
            mockGarbageCollection.Setup(x => x.Add(It.IsAny<GarbageCollection>())).Callback<GarbageCollection>((g) => new List<GarbageCollection>().Add(g));

            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Companies).ReturnsDbSet(companies);
            mockContext.Setup(c => c.GarbageTypes).ReturnsDbSet(garbageTypes);
            mockContext.Setup(c => c.GarbageCollections).Returns(mockGarbageCollection.Object);
            mockContext.Setup(c => c.CompanyGarbageCollections).ReturnsDbSet(new List<CompanyGarbageCollection>());
            mockContext.Setup(c => c.GarbageCollectionGarbageTypes).ReturnsDbSet(new List<GarbageCollectionGarbageType>());

            var garbageCollectionRepository = new GarbageCollectionRepository(mockContext.Object);
            var garbageCollectionController = new GarbageCollectionsController(garbageCollectionRepository);

            var request = new CreateGarbageCollectionRequest() { CompanyId = _companyId, GarbageTypes = testGarbageTypes, CollectionTime = _collectionTime};
            var result = await garbageCollectionController.Post(request);

            Assert.IsNotNull(result);

            mockGarbageCollection.Verify(m => m.Add(It.IsAny<GarbageCollection>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
        [TestMethod]
        public async Task GetGarbageCollectionsWithGarbageTypesForWeek_ShouldReturnCorrectCollections()
        {
            var getGarbageCollectionRequest = new GetGarbageCollectionRequest()
                { CompanyId = _companyId, Date = _collectionTime };

            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Companies).ReturnsDbSet(GetFakeCompanies());
            mockContext.Setup(c => c.GarbageTypes).ReturnsDbSet(GetFakeGarbageTypes());
            mockContext.Setup(c => c.GarbageCollections).ReturnsDbSet(GetFakeGarbageCollections());
            mockContext.Setup(c => c.CompanyGarbageCollections).ReturnsDbSet(GetFakeCompanyGarbageCollections());
            mockContext.Setup(c => c.GarbageCollectionGarbageTypes).ReturnsDbSet(GetFakeGarbageCollectionGarbageType());

            var garbageCollectionRepository = new GarbageCollectionRepository(mockContext.Object);
            var garbageCollectionController = new GarbageCollectionsController(garbageCollectionRepository);

            var result = await garbageCollectionController.GetForWeek(getGarbageCollectionRequest);
            var value = (result as OkObjectResult)?.Value as List<GarbageCollectionGarbageTypeResponse>;

            Assert.IsNotNull(result);
            Assert.IsNotNull(value);
            Assert.AreEqual(2, value.Count);
        }
        [TestMethod]
        public async Task GetGarbageCollectionsWithGarbageTypesForTimeSlot_ShouldReturnCorrectTimeSlot()
        {
            var getGarbageCollectionRequest = new GetGarbageCollectionRequest()
                { CompanyId = _companyId, Date = _collectionTime };

            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Companies).ReturnsDbSet(GetFakeCompanies());
            mockContext.Setup(c => c.GarbageTypes).ReturnsDbSet(GetFakeGarbageTypes());
            mockContext.Setup(c => c.GarbageCollections).ReturnsDbSet(GetFakeGarbageCollections());
            mockContext.Setup(c => c.CompanyGarbageCollections).ReturnsDbSet(GetFakeCompanyGarbageCollections());
            mockContext.Setup(c => c.GarbageCollectionGarbageTypes).ReturnsDbSet(GetFakeGarbageCollectionGarbageType());

            var garbageCollectionRepository = new GarbageCollectionRepository(mockContext.Object);
            var garbageCollectionController = new GarbageCollectionsController(garbageCollectionRepository);

            var result = await garbageCollectionController.GetForTimeSlot(getGarbageCollectionRequest);
            var value = (result as OkObjectResult)?.Value as GarbageCollectionGarbageTypeResponse;

            Assert.IsNotNull(result);

            Assert.IsNotNull(value);
            Assert.AreEqual(_collectionTime, value.CollectionTime);
            Assert.AreEqual(1, value.GarbageTypes.Count);

            Assert.IsTrue(value.GarbageTypes.Contains("REST"));
        }
        [TestMethod]
        public async Task GetGarbageCollectionsWithGarbageTypesForTimeSlot_ShouldReturnNoTimeSlot()
        {
            var getGarbageCollectionRequest = new GetGarbageCollectionRequest()
                { CompanyId = _companyId, Date = DateTime.UtcNow }; // Datetime will never match the collectionTime set for that company

            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.CompanyGarbageCollections).ReturnsDbSet(GetFakeCompanyGarbageCollections());

            var garbageCollectionRepository = new GarbageCollectionRepository(mockContext.Object);
            var garbageCollectionController = new GarbageCollectionsController(garbageCollectionRepository);

            var result = await garbageCollectionController.GetForTimeSlot(getGarbageCollectionRequest);
            var value = result as BadRequestObjectResult;
            
            Assert.IsNotNull(result);
            Assert.IsNotNull(value);
            Assert.AreEqual("No garbageCollections found", value.Value);
        }
        [TestMethod]
        public async Task DeleteGarbageCollection_ShouldReturnOkResult()
        {
            var deleteGarbageCollectionRequest = new DeleteGarbageCollectionRequest()
                { CompanyId = _companyId, CollectionTime = _collectionTime};

            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Companies).ReturnsDbSet(GetFakeCompanies());
            mockContext.Setup(c => c.GarbageTypes).ReturnsDbSet(GetFakeGarbageTypes());
            mockContext.Setup(c => c.GarbageCollections).ReturnsDbSet(GetFakeGarbageCollections());
            mockContext.Setup(c => c.CompanyGarbageCollections).ReturnsDbSet(GetFakeCompanyGarbageCollections());
            mockContext.Setup(c => c.GarbageCollectionGarbageTypes).ReturnsDbSet(GetFakeGarbageCollectionGarbageType());

            var garbageCollectionRepository = new GarbageCollectionRepository(mockContext.Object);
            var garbageCollectionController = new GarbageCollectionsController(garbageCollectionRepository);

            var result = await garbageCollectionController.Delete(deleteGarbageCollectionRequest);
            var value = result as OkObjectResult;

            Assert.IsNotNull(result);

            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        [TestMethod]
        public async Task DeleteGarbageCollection_ShouldReturnBadRequest()
        {
            var deleteGarbageCollectionRequest = new DeleteGarbageCollectionRequest()
                { CompanyId = _companyId, CollectionTime = _collectionTime };

            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Companies).ReturnsDbSet(new List<Company>()); // no companies given

            var garbageCollectionRepository = new GarbageCollectionRepository(mockContext.Object);
            var garbageCollectionController = new GarbageCollectionsController(garbageCollectionRepository);

            var result = await garbageCollectionController.Delete(deleteGarbageCollectionRequest);
            var value = result as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Company not found", value?.Value);
        }

        private List<Company> GetFakeCompanies()
        {
            return new List<Company>() { new() { Address = "Test", Id = _companyId, Name = "TestCompany" } };
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
            return new List<GarbageCollection>() { new() {Id = _garbageCollectionId1, CollectionTime = _collectionTime}, new() { Id = _garbageCollectionId2, CollectionTime = _collectionTime.AddDays(2)} };
        }
        private List<CompanyGarbageCollection>GetFakeCompanyGarbageCollections()
        {
            var fakeCompanies = GetFakeCompanies();
            var fakeGarbageCollections = GetFakeGarbageCollections();
            return new List<CompanyGarbageCollection>() { new() { Company = fakeCompanies[0], CompanyId = _companyId, GarbageCollectionId = _garbageCollectionId1, GarbageCollection = fakeGarbageCollections[0]}, new() { Company = fakeCompanies[0], CompanyId = _companyId, GarbageCollection = fakeGarbageCollections[1], GarbageCollectionId = _garbageCollectionId2 } };
        }
        private List<GarbageCollectionGarbageType> GetFakeGarbageCollectionGarbageType()
        {
            return new List<GarbageCollectionGarbageType>() { new() { GarbageTypeId = 1, GarbageCollectionId = _garbageCollectionId1 }, new() { GarbageTypeId = 2, GarbageCollectionId = _garbageCollectionId2 } };
        }
    }
}
