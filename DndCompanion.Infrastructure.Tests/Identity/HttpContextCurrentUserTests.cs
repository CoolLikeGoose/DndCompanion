using System.Security.Claims;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Http;

namespace DndCompanion.Infrastructure.Tests.Identity;

public class HttpContextCurrentUserTests
{
    private static HttpContextCurrentUser CreateWithHttpContext(HttpContext? httpContext)
    {
        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        return new HttpContextCurrentUser(accessor);
    }

    [Fact]
    public void IsAuthenticated_ReturnsFalse_WhenNoHttpContext()
    {
        var currentUser = CreateWithHttpContext(null);
        Assert.False(currentUser.IsAuthenticated);
    }

    [Fact]
    public void IsAuthenticated_ReturnsFalse_WhenUnauthenticatedIdentity()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity())
        };

        var currentUser = CreateWithHttpContext(httpContext);

        Assert.False(currentUser.IsAuthenticated);
    }

    [Fact]
    public void IsAuthenticated_ReturnsTrue_WhenAuthenticatedIdentity()
    {
        var userId = Guid.NewGuid();
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, "TestUser")
        ], authenticationType: "TestAuth");

        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        var currentUser = CreateWithHttpContext(httpContext);

        Assert.True(currentUser.IsAuthenticated);
    }

    [Fact]
    public void UserId_ReturnsParsedGuid_WhenClaimPresent()
    {
        var userId = Guid.NewGuid();
        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, userId.ToString())],
            authenticationType: "TestAuth");

        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        var currentUser = CreateWithHttpContext(httpContext);

        Assert.Equal(userId, currentUser.UserId);
    }

    [Fact]
    public void UserId_ReturnsNull_WhenClaimMissing()
    {
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) };
        var currentUser = CreateWithHttpContext(httpContext);

        Assert.Null(currentUser.UserId);
    }

    [Fact]
    public void UserId_ReturnsNull_WhenClaimNotGuid()
    {
        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, "not-a-guid")],
            authenticationType: "TestAuth");

        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        var currentUser = CreateWithHttpContext(httpContext);

        Assert.Null(currentUser.UserId);
    }

    [Fact]
    public void UserName_ReturnsClaimValue_WhenPresent()
    {
        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, "TestUser")],
            authenticationType: "TestAuth");

        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        var currentUser = CreateWithHttpContext(httpContext);

        Assert.Equal("TestUser", currentUser.UserName);
    }
}