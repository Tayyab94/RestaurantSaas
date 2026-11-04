using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace RestaurantSaaS.IntegrationTests.Api;

/// <summary>
/// API rules that do not need a database: the API is closed by default and bad input is refused before any query.
/// Sign-in against a real database is tested with the Testcontainers suite (PostgreSQL and SQL Server).
/// </summary>
public class AuthApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Theory]
    [InlineData("/api/v1/dashboard/summary")]
    [InlineData("/api/v1/users")]
    [InlineData("/api/v1/tenants")]
    [InlineData("/api/v1/roles")]
    public async Task Secured_endpoints_answer_401_without_a_token(string path)
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(path);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Sign_in_with_empty_fields_is_refused_with_a_validation_error()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { businessCode = "", userName = "", password = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Registration_with_a_weak_password_is_refused()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            businessName = "Test Cafe",
            businessType = "Cafe",
            ownerFullName = "Owner",
            userName = "owner",
            password = "weak",
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
