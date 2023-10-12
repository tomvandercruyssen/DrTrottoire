using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DrTrottoirApi.Controllers;
using DrTrottoirApi.Entities;
using DrTrottoirApi.Models;
using DrTrottoirApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Tests.Controllers
{
    [TestClass]
    public class TestGarbageTypeController
    {
        [TestMethod]
        public async Task GetAllGarbageTypes_ShouldReturnAllGarbageTypes()
        {
            var garbageTypes = GetTestGarbageTypes();
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.GarbageTypes).ReturnsDbSet(garbageTypes);

            var garbageTypeRepository = new GarbageTypeRepository(mockContext.Object);
            var garbageTypeController = new GarbageTypesController(garbageTypeRepository);

            var result = await garbageTypeController.Get();
            var actionResult = result.Result as OkObjectResult;
            var response = actionResult?.Value as IList<GarbageTypeResponse>;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(200, actionResult.StatusCode);

            Assert.IsNotNull(response);
            Assert.AreEqual(garbageTypes.Count, response.Count);
        }

        [TestMethod]
        public async Task CreateGarbageType_ShouldReturnOkResult()
        {
            var garbageTypeRequest = new CreateGarbageTypeRequest() { Name = "TESTGFT" };
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(m => m.GarbageTypes).ReturnsDbSet(GetTestGarbageTypes());

            var garbageTypeRepository = new GarbageTypeRepository(mockContext.Object);
            var garbageTypeController = new GarbageTypesController(garbageTypeRepository);

            var result = await garbageTypeController.Post(garbageTypeRequest);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task CreateGarbageType_ShouldReturnAlreadyExist()
        {
            var garbageTypeRequest = new CreateGarbageTypeRequest() { Name = "GFT" };
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(m => m.GarbageTypes).ReturnsDbSet(GetTestGarbageTypes());

            var garbageTypeRepository = new GarbageTypeRepository(mockContext.Object);
            var garbageTypeController = new GarbageTypesController(garbageTypeRepository);

            var result = await garbageTypeController.Post(garbageTypeRequest);
            var TextValue = (result as BadRequestObjectResult).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual("GarbageType already exists", TextValue);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task CreateGarbageType_ShouldReturnBadRequest()
        {
            var garbageTypeRequest = new CreateGarbageTypeRequest() { Name = null };

            var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<GarbageType>>();

            var mockContext = new Mock<DrTrottoirDbContext>();
            var garbageTypeRepository = new GarbageTypeRepository(mockContext.Object);
            var garbageTypeController = new GarbageTypesController(garbageTypeRepository);

            var result = await garbageTypeController.Post(garbageTypeRequest);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        private List<GarbageType> GetTestGarbageTypes()
        {
            var garbageTypes = new[]
            {
                new GarbageType() { Name = "REST" },
                new GarbageType() { Name = "GLAS" },
                new GarbageType() { Name = "PAPIER" },
                new GarbageType() { Name = "PMD" },
                new GarbageType() { Name = "GFT" },
            };

            return garbageTypes.ToList();
        }
    }
}