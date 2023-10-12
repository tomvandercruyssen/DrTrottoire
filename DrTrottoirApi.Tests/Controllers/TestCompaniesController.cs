using DrTrottoirApi.Controllers;
using DrTrottoirApi.Entities;
using DrTrottoirApi.Models;
using DrTrottoirApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Tests.Controllers
{
    [TestClass]
    public class CompanyControllerTests
    {
        private readonly Guid _company1Guid = Guid.NewGuid();
        private readonly Guid _company2Guid = Guid.NewGuid();
        [TestMethod]
        public async Task GetAllCompanies_ShouldReturnListOfCompanies()
        {
            var companies = GetFakeCompanies();
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Companies).ReturnsDbSet(companies);

            var companyRepository = new CompanyRepository(mockContext.Object);
            var companyController = new CompaniesController(companyRepository);

            var result = await companyController.Get();
            var actionResult = result.Result as OkObjectResult;
            var response = actionResult?.Value as IList<Company>;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(200, actionResult.StatusCode);

            Assert.IsNotNull(response);
            Assert.AreEqual(companies.Count, response.Count);

        }

        [TestMethod]
        public async Task GetCompanyById_ShouldReturnCompany()
        {
            var companies = GetFakeCompanies();
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Companies).ReturnsDbSet(companies);

            var companyRepository = new CompanyRepository(mockContext.Object);
            var companyController = new CompaniesController(companyRepository);

            var result = await companyController.Get(_company1Guid);
            var actionResult = result.Value;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(companies[0], actionResult);
        }

        [TestMethod]
        public async Task GetCompanyById_ShouldReturnBadRequest()
        {
            var mockContext = new Mock<DrTrottoirDbContext>();
            var companyRepository = new CompanyRepository(mockContext.Object);
            var companyController = new CompaniesController(companyRepository);

            var result = await companyController.Get(_company1Guid);
            var actionResult = result.Result as BadRequestObjectResult;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(400, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateCompany_ShouldReturnBadRequest()
        {
            var companies = GetFakeCompanies();
            var companyRequest = new CreateCompanyRequest() { Address = companies[0].Address, IdKbo = companies[0].IdKbo, SyndicId = companies[0].Syndic.Id };
            var syndics = new List<Syndic>() { companies[0].Syndic, companies[1].Syndic };

            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Companies).ReturnsDbSet(companies);
            mockContext.Setup(c => c.Syndics).ReturnsDbSet(syndics);

            var repo = new CompanyRepository(mockContext.Object);
            var controller = new CompaniesController(repo);

            var putResult = await controller.Put(_company2Guid, companyRequest);

            Assert.IsNotNull(putResult);
            Assert.IsInstanceOfType(putResult, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task DeleteCompany_ShouldReturnOk()
        {
            var companies = GetFakeCompanies();
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Companies).ReturnsDbSet(companies);

            var companiesRepository = new CompanyRepository(mockContext.Object);
            var controller = new CompaniesController(companiesRepository);

            var deleteResult = await controller.Delete(Guid.NewGuid());

            Assert.IsNotNull(deleteResult);
            Assert.IsInstanceOfType(deleteResult, typeof(BadRequestObjectResult));
        }
        [TestMethod]
        public async Task GetCoordinates_ShouldReturnCorrectValues()
        {
            var companies = GetFakeCompanies();
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Companies).ReturnsDbSet(companies);

            var companiesRepository = new CompanyRepository(mockContext.Object);
            var controller = new CompaniesController(companiesRepository);

            var deleteResult = await controller.GetCoordinates(_company1Guid);
            var value = (deleteResult as OkObjectResult).Value;
           
            Assert.IsNotNull(value);
            Assert.AreEqual((3.9875008, 51.1065052), value);
        }
        [TestMethod]
        public async Task GetCoordinates_ShouldReturnCompanyNotFoundException()
        {
            var companies = GetFakeCompanies();
            var mockContext = new Mock<DrTrottoirDbContext>();
            mockContext.Setup(c => c.Companies).ReturnsDbSet(companies);

            var companiesRepository = new CompanyRepository(mockContext.Object);
            var controller = new CompaniesController(companiesRepository);

            var deleteResult = await controller.GetCoordinates(Guid.NewGuid());
            var value = (deleteResult as BadRequestObjectResult).Value;

            Assert.IsNotNull(value);
            Assert.AreEqual("Company not found", value);
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

    }
}
