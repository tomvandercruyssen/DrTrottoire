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
using Task = DrTrottoirApi.Entities.Task;

namespace DrTrottoirApi.Tests.Controllers
{
    [TestClass]
    public class TestTaskController
    {
        private readonly Guid _task1Guid = Guid.NewGuid();
        private readonly Guid _task2Guid = Guid.NewGuid();

        private readonly Guid _company1Guid = Guid.NewGuid();
        private readonly Guid _company2Guid = Guid.NewGuid();

        private readonly Guid _roundId = Guid.NewGuid();

        [TestMethod]
        public async System.Threading.Tasks.Task GetAllTasks_ShouldReturnAllTasks()
        {
            var tasks = GetFakeTasks();
            var pictures = GetPictures();
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Tasks).ReturnsDbSet(tasks);
            mockContext.Setup(p => p.Pictures).ReturnsDbSet(pictures);
            mockContext.Setup(p => p.Companies).ReturnsDbSet(GetFakeCompanies());

            var taskRepository = new TaskRepository(mockContext.Object);
            var taskController = new TasksController(taskRepository);

            var result = await taskController.Get();
            var actionResult = result.Result as OkObjectResult;
            var response = actionResult?.Value as IList<BaseTaskResponse>;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(200, actionResult.StatusCode);

            Assert.IsNotNull(response);
            Assert.AreEqual(tasks.Count, response.Count);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task GetTaskById_ShouldReturnBadRequest()
        {
            var mockContext = new Mock<DrTrottoirDbContext>();
            var taskRepository = new TaskRepository(mockContext.Object);
            var taskController = new TasksController(taskRepository);

            var result = await taskController.Get(_task1Guid);
            var actionResult = result.Result as BadRequestObjectResult;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(400, actionResult.StatusCode);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task GetTaskById_ShouldReturnTheCorrectTask()
        {
            var tasks = GetFakeTasks();
            var pictures = GetPictures();
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Tasks).ReturnsDbSet(tasks);
            mockContext.Setup(p => p.Pictures).ReturnsDbSet(pictures);
            mockContext.Setup(p => p.Companies).ReturnsDbSet(GetFakeCompanies());

            var taskRepository = new TaskRepository(mockContext.Object);
            var taskController = new TasksController(taskRepository);

            var result = await taskController.Get(_task1Guid);
            var response = result.Value as BaseTaskResponse;

            Assert.IsNotNull(response);
            Assert.AreEqual(tasks[0].Id, response.Id);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task DeleteTask_ShouldReturnBadRequest()
        {
            var tasks = GetFakeTasks();
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Tasks).ReturnsDbSet(tasks);

            var taskRepository = new TaskRepository(mockContext.Object);
            var taskController = new TasksController(taskRepository);

            var deleteResult = await taskController.Delete(Guid.NewGuid());

            Assert.IsNotNull(deleteResult);
            Assert.IsInstanceOfType(deleteResult, typeof(BadRequestObjectResult));
        }


        [TestMethod]
        public async System.Threading.Tasks.Task CreateTask_ShouldReturnOkResult()
        {
            var taskRequest = new CreateTaskRequest() { CompanyId = _company1Guid, RoundId = _roundId };

            var tasks = GetFakeTasks();
            var companies = GetFakeCompanies();
            var companyRequest = new CreateCompanyRequest() { Address = companies[0].Address, IdKbo = companies[0].IdKbo, SyndicId = companies[0].Syndic.Id };
            var syndics = new List<Syndic>() { companies[0].Syndic, companies[1].Syndic };
            var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<Task>>();
            mockSet.Setup(x => x.Add(It.IsAny<Task>())).Callback<Task>((s) => tasks.Add(s));

            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(m => m.Tasks).Returns(mockSet.Object); 
            mockContext.Setup(c => c.Companies).ReturnsDbSet(companies);
            mockContext.Setup(c => c.Syndics).ReturnsDbSet(syndics);
            mockContext.Setup(c => c.Rounds).ReturnsDbSet(GetFakeRounds());

            var repo = new TaskRepository(mockContext.Object);
            var controller = new TasksController(repo);
            await controller.Post(taskRequest);

            mockSet.Verify(m => m.Add(It.IsAny<Task>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task DeleteTask_ShouldReturnOkObjectResult()
        {
            var tasks = GetFakeTasks();
            var pictures = GetPictures();
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Tasks).ReturnsDbSet(tasks);
            mockContext.Setup(p => p.Pictures).ReturnsDbSet(pictures);

            var taskRepository = new TaskRepository(mockContext.Object);
            var taskController = new TasksController(taskRepository);

            var deleteResult = await taskController.Delete(_task1Guid);
            var actionResult = deleteResult as OkObjectResult;
            Assert.IsNotNull(deleteResult);

            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task PutTask_ShouldReturnOkObjectResult()
        {
            var tasks = GetFakeTasks();
            var companies = GetFakeCompanies();
            var syndics = new List<Syndic>() { companies[0].Syndic, companies[1].Syndic };


            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Tasks).ReturnsDbSet(tasks);
            mockContext.Setup(c => c.Companies).ReturnsDbSet(companies);
            mockContext.Setup(c => c.Syndics).ReturnsDbSet(syndics);
            mockContext.Setup(c => c.Rounds).ReturnsDbSet(GetFakeRounds());
            var request = new CreateTaskRequest() { CompanyId = _company1Guid, RoundId = _roundId };
            var taskRepository = new TaskRepository(mockContext.Object);
            var taskController = new TasksController(taskRepository);

            var putResult = await taskController.Put(_task2Guid, request);

            Assert.IsNotNull(putResult);
            Assert.IsInstanceOfType(putResult, typeof(OkResult));

            mockContext.Verify(m => m.Tasks.Update(It.IsAny<Task>()), Times.Once());
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task PutTask_ShouldReturnBadRequest()
        {
            var tasks = GetFakeTasks();

            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Tasks).ReturnsDbSet(tasks);

            var request = new CreateTaskRequest() { CompanyId = Guid.NewGuid(), RoundId = Guid.NewGuid() };
            var taskRepository = new TaskRepository(mockContext.Object);
            var taskController = new TasksController(taskRepository);

            var putResult = await taskController.Put(Guid.NewGuid(), request);

            Assert.IsNotNull(putResult);
            Assert.IsInstanceOfType(putResult, typeof(BadRequestObjectResult));

            mockContext.Verify(m => m.Tasks.Update(It.IsAny<Task>()), Times.Never);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        private List<Task> GetFakeTasks()
        {
            var companies = GetFakeCompanies();
            var round = GetFakeRounds();
            return new List<Task>()
            {
                new() { Id = _task1Guid, StartTime = DateTime.Now.AddHours(-5), EndTime = DateTime.Now, Company = companies[0], Round = round[0]},
                new() { Id = _task2Guid, StartTime = DateTime.Now.AddHours(-10), EndTime = DateTime.Now, Company = companies[0], Round = round[0]}
            };
        }

        private List<Picture> GetPictures()
        {
            return new List<Picture>()
            {
                new() { PictureLabel = PictureLabel.Arrival, PictureUrl = "testfoto", TaskId = _task1Guid },
                new() { PictureLabel = PictureLabel.Departure, PictureUrl = "testfoto2", TaskId = _task1Guid }
            };
        }

        private List<Company> GetFakeCompanies()
        {
            List<Syndic> syndics = new List<Syndic>()
            {
                new() { Id = Guid.NewGuid(), FirstName = "Laurens", LastName = "Test", TelephoneNumber = "0486976431" },
                new() { Id = Guid.NewGuid(), FirstName = "Tom", LastName = "Test", TelephoneNumber = "0486976431" }
            };
            return new List<Company>()
            {
                new() { Id = _company1Guid, Address = "koningslaan 1", IdKbo = "1234.567.891" , Name = "UpKot", Syndic = syndics[0], Longitude = 3.9875008, Latitude = 51.1065052},
                new() { Id = _company2Guid, Address = "Gebroedersdesmetstraat 54", IdKbo = "4567.891.234", Name = "TechnologieCampus", Syndic = syndics[1] }
            };
        }

        private List<Round> GetFakeRounds()
        {
            return new List<Round>()
            {
                new() { Id = _roundId, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(3)}
            };
        }
    }
}
