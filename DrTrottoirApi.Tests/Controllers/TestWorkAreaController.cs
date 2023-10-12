using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class TestWorkAreaController
    {
        private readonly Guid _workAreaGuid1 = Guid.NewGuid();
        private readonly Guid _workAreaGuid2 = Guid.NewGuid();

        [TestMethod]
        public async Task GetAllWorkAreas_ShouldReturnAllWorkAreas()
        {
            var workAreas = GetFakeWorkAreas();
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.WorkAreas).ReturnsDbSet(workAreas);

            var workAreaRepository = new WorkAreaRepository(mockContext.Object);
            var workAreaController = new WorkAreasController(workAreaRepository);

            var result = await workAreaController.Get();
            var actionResult = result as OkObjectResult;
            var response = actionResult?.Value as IList<WorkArea>;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(200, actionResult.StatusCode);

            Assert.IsNotNull(response);
            Assert.AreEqual(workAreas.Count, response.Count);
        }
        
        [TestMethod]
        public async Task GetWorkAreaById_ShouldReturnBadRequest()
        {
            var mockContext = new Mock<DrTrottoirDbContext>();
            var workAreaRepository = new WorkAreaRepository(mockContext.Object);
            var workAreaController = new WorkAreasController(workAreaRepository);

            var result = await workAreaController.Get(_workAreaGuid1);
            var actionResult = result as BadRequestObjectResult;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(400, actionResult.StatusCode);
        }
        
        [TestMethod]
        public async Task GetWorkAreaById_ShouldReturnTheCorrectWorkArea()
        {
            var workAreas = GetFakeWorkAreas();
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.WorkAreas).ReturnsDbSet(workAreas);

            var workAreaRepository = new WorkAreaRepository(mockContext.Object);
            var workAreaController = new WorkAreasController(workAreaRepository);

            var result = await workAreaController.Get(_workAreaGuid2);
            var actionResult = result as OkObjectResult;
            var response = actionResult?.Value as WorkArea;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(200, actionResult.StatusCode);

            Assert.IsNotNull(response);
            Assert.AreEqual(workAreas[1], response);
        }
        
        [TestMethod]
        public async Task DeleteWorkArea_ShouldReturnBadRequest()
        {
            var workAreas = GetFakeWorkAreas();
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.WorkAreas).ReturnsDbSet(workAreas);

            var workAreaRepository = new WorkAreaRepository(mockContext.Object);
            var workAreaController = new WorkAreasController(workAreaRepository);

            var deleteResult = await workAreaController.Delete(Guid.NewGuid());

            Assert.IsNotNull(deleteResult);
            Assert.IsInstanceOfType(deleteResult, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task CreateWorkArea_ShouldReturnOkResult()
        {
            var workAreaRequest = new CreateWorkAreaRequest() { City = "Parijs"};

            var workAreas = GetFakeWorkAreas();

            var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<WorkArea>>();
            mockSet.Setup(x => x.Add(It.IsAny<WorkArea>())).Callback<WorkArea>((w) => workAreas.Add(w));

            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(m => m.WorkAreas).Returns(mockSet.Object);

            var workAreaRepository = new WorkAreaRepository(mockContext.Object);
            var workAreaController = new WorkAreasController(workAreaRepository);

            await workAreaController.Post(workAreaRequest);

            mockSet.Verify(m => m.Add(It.IsAny<WorkArea>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
        
        [TestMethod]
        public async Task DeleteWorkArea_ShouldReturnOkObjectResult()
        {
            var workAreas = GetFakeWorkAreas();
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.WorkAreas).ReturnsDbSet(workAreas);

            var workAreaRepository = new WorkAreaRepository(mockContext.Object);
            var workAreaController = new WorkAreasController(workAreaRepository);

            var deleteResult = await workAreaController.Delete(_workAreaGuid1);

            Assert.IsNotNull(deleteResult);
            Assert.IsInstanceOfType(deleteResult, typeof(OkResult));

            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
        }
        
        [TestMethod]
        public async Task PutWorkArea_ShouldReturnOkObjectResult()
        {
            var workAreas = GetFakeWorkAreas();

            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.WorkAreas).ReturnsDbSet(workAreas);

            var request = new CreateWorkAreaRequest() { City = "Oostende" };

            var workAreaRepository = new WorkAreaRepository(mockContext.Object);
            var workAreaController = new WorkAreasController(workAreaRepository);

            var putResult = await workAreaController.Put(_workAreaGuid2, request);

            Assert.IsNotNull(putResult);
            Assert.IsInstanceOfType(putResult, typeof(OkResult));

            mockContext.Verify(m => m.WorkAreas.Update(It.IsAny<WorkArea>()), Times.Once());
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
        }
        
        [TestMethod]
        public async Task PutWorkArea_ShouldReturnBadRequest()
        {
            var workAreas = GetFakeWorkAreas();

            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.WorkAreas).ReturnsDbSet(workAreas);

            var request = new CreateWorkAreaRequest() { City = "Oostende" };

            var workAreaRepository = new WorkAreaRepository(mockContext.Object);
            var workAreaController = new WorkAreasController(workAreaRepository);

            var putResult = await workAreaController.Put(Guid.NewGuid(), request);

            Assert.IsNotNull(putResult);
            Assert.IsInstanceOfType(putResult, typeof(BadRequestObjectResult));

            mockContext.Verify(m => m.WorkAreas.Update(It.IsAny<WorkArea>()), Times.Never);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
        
        private List<WorkArea> GetFakeWorkAreas()
        {
            return new List<WorkArea>()
            {
                new() { Id = _workAreaGuid1, City = "Oostende"},
                new() { Id = _workAreaGuid2, City = "Luik" }
            };
        }
    }
}
