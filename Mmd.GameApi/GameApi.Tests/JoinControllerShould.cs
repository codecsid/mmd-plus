using DataAccess.Data;
using DataAccess.Data.Services;
using DataAccess.Model;
using DataAccess.Model.SharedModels;
using FluentAssertions;
using GameApi.Service.Controllers;
using GameApi.Tests.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace GameApi.Tests
{
    public class JoinControllerShould
    {
        private readonly JoinController controller;

        public JoinControllerShould()
        {
            var dataContext = new Mock<DataContext>();
            var requestContext = new RequestContext();
            var gameApiServiceLogger = new Mock<ILogger<GameApiService>>().Object;
            var gameApiService = new Mock<GameApiService>(dataContext.Object, gameApiServiceLogger, requestContext);
            var requestLoggingService = new RequestLoggingService(requestContext, dataContext.Object);
            var logger = Mock.Of<ILogger<JoinController>>();
            controller = new JoinController(requestContext, gameApiService.Object, requestLoggingService, logger);
        }

        private Mock<ILogger<T>> GetMockLogger<T>()
        {
            return new Mock<ILogger<T>>();
        }

        [Fact]
        public void BeAController()
        {
            Assert.IsAssignableFrom<Controller>(controller);
        }

        [Fact]
        public void HaveAnApiControllerAttribute()
        {
            Attribute.GetCustomAttributes(typeof(JoinController), typeof(ApiControllerAttribute)).Any().Should().BeTrue();
        }

        [Fact]
        public void ProduceApplicationJsonResponse()
        {
            var producesAttribute = (ProducesAttribute)Attribute.GetCustomAttribute(typeof(JoinController), typeof(ProducesAttribute));
            producesAttribute.Should().NotBeNull();
            producesAttribute.ContentTypes.Should().Contain("application/json");
        }

        [Fact]
        public void HaveAnAuthorizationAttributeForBasicAuth()
        {
            var authorizeAttribute = (AuthorizeAttribute)Attribute.GetCustomAttribute(typeof(JoinController), typeof(AuthorizeAttribute));
            authorizeAttribute.Should().NotBeNull();
            authorizeAttribute.AuthenticationSchemes.Should().Be("BasicAuth");
        }

        [Fact]
        public void HaveAnHttpPostMethod()
        {
            var hasAHttpPostMethod = TestFunctions.MethodHasAttribute(() => controller.Post(), typeof(HttpPostAttribute));
            hasAHttpPostMethod.Should().BeTrue();
        }

        [Fact]
        public async Task ReturnJoinResponse()
        {
            var response = await controller.Post();
            var result = response.Value;
            result.Should().BeAssignableTo<JoinResponse>();
        }

        [Fact]
        public async Task HaveAValidRequestId()
        {
            var response = await controller.Post();
            var result = response.Value;
            result.Should().BeAssignableTo<JoinResponse>();
            ((JoinResponse)result).RequestId.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task ReturnErrorObject_GivenNoRunningGameIsFound()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(ClaimTypes.NameIdentifier, "SomeValueHere"),
                                        new Claim(ClaimTypes.Name, "gunnar@somecompany.com")
                                        // other required and custom claims
                                   }, "BasicAuth"));
            var mockPhases = new List<Phase>().AsQueryable().BuildMockDbSet();
            var dataContext = new Mock<DataContext>();
            dataContext.SetupGet(x => x.Phases).Returns(mockPhases.Object);
            var requestContext = new RequestContext();
            var gameApiServiceLogger = GetMockLogger<GameApiService>().Object;
            var gameApiService = new Mock<GameApiService>(dataContext.Object, gameApiServiceLogger, requestContext) { CallBase = true }.Object;
            var requestLoggingService = new RequestLoggingService(requestContext, dataContext.Object);
            var logger = GetMockLogger<JoinController>().Object;
            var modController = new JoinController(requestContext, gameApiService, requestLoggingService, logger);
            modController.ControllerContext = new ControllerContext();
            modController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
            var response = await modController.Post();
            response.StatusCode.Should().Be(200);
            var result = (JoinResponse)response.Value;
            result.Err.Should().NotBeNull();
            result.Err.Description.Should().Be("Game is not running");
        }

        [Fact]
        public async Task TestServerCreate()
        {
            var testServer = new TestServer(new WebHostBuilder().UseStartup<TestStartup>());
            var mockPhases = new List<Phase>().AsQueryable().BuildMockDbSet();
            var client = testServer.CreateClient();
            var response = await client.GetAsync("/api/gamestatus");
            //response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var gamestatus = JsonConvert.DeserializeObject<GameStatusResponse>(result);
        }
    }
}
