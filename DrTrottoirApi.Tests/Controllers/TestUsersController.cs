using System;
using System.Collections.Generic;
using DrTrottoirApi.Controllers;
using DrTrottoirApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrTrottoirApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Tests
{
    [TestClass]
    public class TestUsersController
    {
        private readonly Guid _userGuid = Guid.NewGuid();

        [TestMethod]
        public async Task GetUserById_ShouldReturnTheCorrectUser()
        {
            var baseUserResponse = new BaseUserResponse()
            {
                Id = _userGuid,
                UserName = "Test",
                FirstName = "Test",
                LastName = "Test",
                Email = "Test",
                PictureUrl = "Test",
                TelephoneNumber = "Test",
                WorkArea = null
            };

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.GetUserById(_userGuid)).ReturnsAsync(baseUserResponse);

            var controller = new UsersController(mockUserRepository.Object);
            var result = await controller.Get(_userGuid);
            var actionResult = result.Result as OkObjectResult;

            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(actionResult.Value);
            Assert.AreEqual(200, actionResult.StatusCode);
        }
        [TestMethod]
        public async Task GetAllUsers_ShouldReturnCorrectUsers()
        {
            IList<BaseUserResponse> users = new List<BaseUserResponse>
            {
                new() { UserName = "Laurens", FirstName = "Laurens", TelephoneNumber = "04968472", LastName = "Van Moere", Email = "laurens@odisee.be" },
                new() { UserName = "Tom", FirstName = "Tom", TelephoneNumber = "04968472", LastName = "Test", Email = "tom@odisee.be" }
            };

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.GetAllUsers()).ReturnsAsync(users);
            var controller = new UsersController(mockUserRepository.Object);
            var result = await controller.Get();
            var actionResult = result.Result as OkObjectResult;
            var response = actionResult?.Value as IList<BaseUserResponse>;

            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(response);
            Assert.IsNotNull(actionResult.StatusCode);
            Assert.AreEqual(users.Count, response.Count);
            Assert.AreEqual(200, actionResult.StatusCode);
        }
        [TestMethod]
        public async Task CreateUser_ShouldReturnOkStatus()
        {
            var createUserRequest = new CreateUserRequest()
            {
                UserName = "Test",
                FirstName = "Test",
                LastName = "Test",
                Email = "Test",
                PictureUrl = "Test",
                TelephoneNumber = "Test",
                WorkAreaId = Guid.NewGuid(),
                Roles = new List<string>() { "Student", "SuperStudent" }
            };

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.CreateUser(createUserRequest));

            var controller = new UsersController(mockUserRepository.Object);
            var result = await controller.Post(createUserRequest);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
    }
}