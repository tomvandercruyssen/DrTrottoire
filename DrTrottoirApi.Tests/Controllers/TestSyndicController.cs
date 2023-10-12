using DrTrottoirApi.Controllers;
using DrTrottoirApi.Entities;
using DrTrottoirApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using DrTrottoirApi.Models;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Tests.Controllers
{
    [TestClass]
    public class TestSyndicController
    {
        private readonly Guid _syndic1Guid = Guid.NewGuid();
        private readonly Guid _syndic2Guid = Guid.NewGuid();

        [TestMethod]
        public async Task GetAllSyndics_ShouldReturnAllSyndics()
        {
            var syndics = GetFakeSyndics();
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Syndics).ReturnsDbSet(syndics);

            var syndicRepository = new SyndicRepository(mockContext.Object);
            var syndicController = new SyndicsController(syndicRepository);

            var result = await syndicController.Get();
            var actionResult = result.Result as OkObjectResult;
            var response = actionResult?.Value as IList<Syndic>;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(200, actionResult.StatusCode);

            Assert.IsNotNull(response);
            Assert.AreEqual(syndics.Count, response.Count);
        }

        [TestMethod]
        public async Task GetSyndicById_ShouldReturnBadRequest()
        {
            var mockContext = new Mock<DrTrottoirDbContext>();
            var syndicRepository = new SyndicRepository(mockContext.Object);
            var syndicController = new SyndicsController(syndicRepository);

            var result = await syndicController.Get(_syndic1Guid);
            var actionResult = result.Result as BadRequestObjectResult;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(400, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task GetSyndicById_ShouldReturnTheCorrectSyndic()
        {
            var syndics = GetFakeSyndics();
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Syndics).ReturnsDbSet(syndics);

            var syndicRepository = new SyndicRepository(mockContext.Object);
            var syndicController = new SyndicsController(syndicRepository);

            var result = await syndicController.Get(_syndic1Guid);
            var actionResult = result.Result as OkObjectResult;
            var response = actionResult?.Value as Syndic;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(200, actionResult.StatusCode);

            Assert.IsNotNull(response);
            Assert.AreEqual(syndics[0], response);
        }

        [TestMethod]
        public async Task DeleteSyndic_ShouldReturnBadRequest()
        {
            var syndics = GetFakeSyndics();
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Syndics).ReturnsDbSet(syndics);

            var syndicRepository = new SyndicRepository(mockContext.Object);
            var syndicController = new SyndicsController(syndicRepository);

            var deleteResult = await syndicController.Delete(Guid.NewGuid());

            Assert.IsNotNull(deleteResult);
            Assert.IsInstanceOfType(deleteResult, typeof(BadRequestObjectResult));
        }


        [TestMethod]
        public async Task CreateSyndic_ShouldReturnOkResult()
        {
            var syndicRequest = new CreateSyndicRequest() { FirstName = "Laurens", LastName = "Test", TelephoneNumber = "0486976431" };

            var syndics = GetFakeSyndics();

            var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<Syndic>>();
            mockSet.Setup(x => x.Add(It.IsAny<Syndic>())).Callback<Syndic>((s) => syndics.Add(s));

            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(m => m.Syndics).Returns(mockSet.Object);

            var repo = new SyndicRepository(mockContext.Object);
            var controller = new SyndicsController(repo);
            await controller.Post(syndicRequest);

            mockSet.Verify(m => m.Add(It.IsAny<Syndic>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async Task DeleteSyndic_ShouldReturnOkObjectResult()
        {
            var syndics = GetFakeSyndics();
            var companies = new List<Company> { new() { Id = Guid.NewGuid(), Name = "Wellington", IdKbo = "0884.824.496", Address = "Fortlaan 27-42", Syndic = syndics[0] }};

            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Syndics).ReturnsDbSet(syndics);
            mockContext.Setup(c => c.Companies).ReturnsDbSet(companies);

            var syndicRepository = new SyndicRepository(mockContext.Object);
            var syndicController = new SyndicsController(syndicRepository);

            var deleteResult = await syndicController.Delete(_syndic1Guid);

            Assert.IsNotNull(deleteResult);
            Assert.IsInstanceOfType(deleteResult, typeof(OkResult));

            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task PutSyndic_ShouldReturnOkObjectResult()
        {
            var syndics = GetFakeSyndics();

            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Syndics).ReturnsDbSet(syndics);

            var request = new CreateSyndicRequest()
                { FirstName = "ChangedName", LastName = "Test", TelephoneNumber = "0486976431" };
            var syndicRepository = new SyndicRepository(mockContext.Object);
            var syndicController = new SyndicsController(syndicRepository);

            var putResult = await syndicController.Put(_syndic2Guid, request);

            Assert.IsNotNull(putResult);
            Assert.IsInstanceOfType(putResult, typeof(OkResult));

            mockContext.Verify(m => m.Syndics.Update(It.IsAny<Syndic>()), Times.Once());
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task PutSyndic_ShouldReturnBadRequest()
        {
            var syndics = GetFakeSyndics();

            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Syndics).ReturnsDbSet(syndics);

            var request = new CreateSyndicRequest()
                { FirstName = "ChangedName", LastName = "Test", TelephoneNumber = "0486976431" };
            var syndicRepository = new SyndicRepository(mockContext.Object);
            var syndicController = new SyndicsController(syndicRepository);

            var putResult = await syndicController.Put(Guid.NewGuid(), request);

            Assert.IsNotNull(putResult);
            Assert.IsInstanceOfType(putResult, typeof(BadRequestObjectResult));

            mockContext.Verify(m => m.Syndics.Update(It.IsAny<Syndic>()), Times.Never);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        private List<Syndic> GetFakeSyndics()
        {
            return new List<Syndic>()
            {
                new() { Id = _syndic1Guid, FirstName = "Laurens", LastName = "Test", TelephoneNumber = "0486976431" },
                new() { Id = _syndic2Guid, FirstName = "Tom", LastName = "Test", TelephoneNumber = "0486976431" }
            };
        }
    }
}
