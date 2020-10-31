using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Castle.Core.Logging;
using DataAccess.Data;
using DataAccess.Data.Services;
using DataAccess.Model;
using DataAccess.Model.SharedModels;
using FluentAssertions;
using GameApi.Service.Controllers;
using GameApi.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace GameApi.Tests
{
    public class GameStatusControllerShould
    {
        private readonly GameStatusController controller;

        public GameStatusControllerShould()
        {
            var dataContext = new Mock<DataContext>();
            var requestContext = new RequestContext();
            var gameApiServiceLogger = new Mock<ILogger<GameApiService>>().Object;
            var gameApiService = new Mock<GameApiService>(dataContext.Object, gameApiServiceLogger, requestContext);
            var requestLoggingService = new RequestLoggingService(requestContext, dataContext.Object);
            var logger = Mock.Of<ILogger<GameStatusController>>();
            controller = new GameStatusController(requestContext, gameApiService.Object, requestLoggingService, logger);
        }

        private Mock<ILogger<T>> GetMockLogger<T>()
        {
            return new Mock<ILogger<T>>();
        }

        [Fact]
        public void BeAControllerBase()
        {
            Assert.IsAssignableFrom<ControllerBase>(controller);
        }

        [Fact]
        public void HaveAnApiControllerAttribute()
        {
            Attribute.GetCustomAttributes(typeof(GameStatusController), typeof(ApiControllerAttribute)).Any().Should().BeTrue();
        }

        [Fact]
        public void ProduceApplicationJsonResponse()
        {
            var producesAttribute = (ProducesAttribute)Attribute.GetCustomAttribute(typeof(GameStatusController), typeof(ProducesAttribute));
            producesAttribute.Should().NotBeNull();
            producesAttribute.ContentTypes.Should().Contain("application/json");
        }

        [Fact]
        public void HaveAnHttpGetMethod()
        {
            var hasAHttpGetMethod = TestFunctions.MethodHasAttribute(() => controller.Get(), typeof( HttpGetAttribute));
            hasAHttpGetMethod.Should().BeTrue();
        }

        [Fact]
        public async Task ReturnGameStatusResponse()
        {
            var response = await controller.Get();
            var result = response.Value;
            result.Should().BeAssignableTo<GameStatusResponse>();
        }

        [Fact]
        public async Task HaveAValidRequestId()
        {
            var response = await controller.Get();
            var result = response.Value;
            result.Should().BeAssignableTo<GameStatusResponse>();
            ((GameStatusResponse)result).RequestId.Should().NotBe(Guid.Empty);
        }

        // GameStatusControllerResponseTests
        
        // 1. If no current game is found, return errorGame is not running, 200
        [Fact]
        public async Task ReturnErrorObject_GivenNoRunningGameIsFound()
        {
            var mockPhases = new List<Phase>().AsQueryable().BuildMockDbSet();
            var dataContext = new Mock<DataContext>();
            dataContext.SetupGet(x => x.Phases).Returns(mockPhases.Object);
            var requestContext = new RequestContext();
            var gameApiServiceLogger = GetMockLogger<GameApiService>().Object;
            var gameApiService = new Mock<GameApiService>(dataContext.Object, gameApiServiceLogger, requestContext) { CallBase = true }.Object;
            var requestLoggingService = new RequestLoggingService(requestContext, dataContext.Object);
            var logger = GetMockLogger<GameStatusController>().Object;
            var modController = new GameStatusController(requestContext, gameApiService, requestLoggingService, logger);
            var response = await modController.Get();
            response.StatusCode.Should().Be(200);
            var result = (GameStatusResponse)response.Value;
            result.Err.Should().NotBeNull();
            result.Err.Description.Should().Be("Game is not running");
        }

        // 2. If current game is there, but no current phase is found, return error try after some time, 200 -- covered above
        // 3. If any exception occurs while getting gamestatus, return error Internal Server Error, 500
        [Fact]
        public async Task Return500_GivenExceptinoOccursInGameApiService()
        {
            var dataContext = new Mock<DataContext>();
            var requestContext = new RequestContext();
            var gameApiServiceLogger = GetMockLogger<GameApiService>().Object;
            var gameApiService = new Mock<GameApiService>(dataContext.Object, gameApiServiceLogger, requestContext);
            gameApiService.Setup(x => x.GetCurrentStatus()).Throws(new ArgumentNullException());
            var requestLoggingService = new RequestLoggingService(requestContext, dataContext.Object);
            var logger = GetMockLogger<GameStatusController>().Object;
            var modController = new GameStatusController(requestContext, gameApiService.Object, requestLoggingService, logger);
            var response = await modController.Get();
            response.StatusCode.Should().Be(500);
            var result = (GameStatusResponse)response.Value;
            result.Err.Should().NotBeNull();
            result.Err.Description.Should().Be("Server Failed to fetch Current Gamestatus");
            result.Err.Message.Should().Be("Internal Server Error");
        }
    }
}