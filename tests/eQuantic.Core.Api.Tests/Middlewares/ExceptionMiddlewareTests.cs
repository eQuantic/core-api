using eQuantic.Core.Api.Middlewares;
using eQuantic.Core.Api.Options;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Moq;

namespace eQuantic.Core.Api.Tests.Middlewares;

public class ExceptionMiddlewareTests
{
    [SetUp]
    public void Setup()
    {
    }

    [TestCaseSource(nameof(ExceptionCases))]
    public async Task ExceptionMiddleware_InvokeAsync_Successfully(Exception exception, int expectedStatusCode)
    {
        var loggerFactory = new Mock<ILoggerFactory>();
        loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(new Mock<ILogger>().Object);
        var httpContext = new DefaultHttpContext();
        var options = new ExceptionFilterOptions();
        
        options.For<ValidationException>().Use(ex =>
            new ExceptionResult(
                StatusCodes.Status400BadRequest,
                ex.Message,
                ex.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    )
            )
        );
        
        var hostEnvironment = new HostingEnvironment();
        var middleware = new ExceptionMiddleware(options, loggerFactory.Object, hostEnvironment);
        await middleware.InvokeAsync(httpContext, _ => throw exception);

        httpContext.Response.StatusCode.Should().Be(expectedStatusCode);
    }
    
    public static object[] ExceptionCases =
    {
        new object[] { new Exception(), StatusCodes.Status500InternalServerError },
        new object[] { new ValidationException(new List<ValidationFailure> { new ("test", "test")}), StatusCodes.Status400BadRequest },
    };
}