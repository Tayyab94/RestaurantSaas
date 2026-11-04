using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using RestaurantSaaS.Contracts.Services;
using RestaurantSaaS.Shared.UI.Services.Auth;

namespace RestaurantSaaS.Shared.UI.Services.Remote;

/// <summary>
/// The one place that talks HTTP to the server API: adds the bearer token, reads JSON,
/// and turns problem responses into ServiceException (the same error the Local services throw).
/// </summary>
internal sealed class RemoteApiClient(HttpClient http, IServiceProvider services)
{
    public Task<T> GetAsync<T>(string path, CancellationToken ct) => SendAsync<T>(HttpMethod.Get, path, null, true, ct);

    public Task<T> GetAsync<T>(string path, bool authenticated, CancellationToken ct) => SendAsync<T>(HttpMethod.Get, path, null, authenticated, ct);

    public async Task PutAsync(string path, object? body, CancellationToken ct)
    {
        using var response = await SendRawAsync(HttpMethod.Put, path, body, true, ct);
        await EnsureSuccessAsync(response, ct);
    }

    public Task<T> PostAsync<T>(string path, object? body, bool authenticated, CancellationToken ct) =>
        SendAsync<T>(HttpMethod.Post, path, body, authenticated, ct);

    public async Task PostAsync(string path, object? body, bool authenticated, CancellationToken ct)
    {
        using var response = await SendRawAsync(HttpMethod.Post, path, body, authenticated, ct);
        await EnsureSuccessAsync(response, ct);
    }

    public async Task DeleteAsync(string path, CancellationToken ct)
    {
        using var response = await SendRawAsync(HttpMethod.Delete, path, null, true, ct);
        await EnsureSuccessAsync(response, ct);
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string path, object? body, bool authenticated, CancellationToken ct)
    {
        using var response = await SendRawAsync(method, path, body, authenticated, ct);
        await EnsureSuccessAsync(response, ct);

        return await response.Content.ReadFromJsonAsync<T>(ct)
               ?? throw new ServiceException("empty_response", "The server returned an empty answer.");
    }

    private async Task<HttpResponseMessage> SendRawAsync(HttpMethod method, string path, object? body, bool authenticated, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(method, path);

        if (body is not null)
        {
            request.Content = JsonContent.Create(body, body.GetType());
        }

        if (authenticated)
        {
            // Resolved late: the session itself uses the (anonymous) auth service of this client.
            var token = await services.GetRequiredService<AuthSession>().GetAccessTokenAsync(ct);

            if (token is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        try
        {
            return await http.SendAsync(request, ct);
        }
        catch (HttpRequestException)
        {
            throw new ServiceException("network", "The server could not be reached. Check the connection and try again.");
        }
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemPayload>(ct);

            if (problem?.Title is not null)
            {
                throw new ServiceException(problem.Type ?? "error", problem.Title, problem.Errors);
            }
        }
        catch (Exception ex) when (ex is not ServiceException)
        {
            // Not a problem document; fall through to the generic message.
        }

        throw new ServiceException(
            response.StatusCode == HttpStatusCode.Unauthorized ? "unauthenticated" : "error",
            $"The server answered with status {(int)response.StatusCode}.");
    }

    private sealed record ProblemPayload(string? Type, string? Title, Dictionary<string, string[]>? Errors);
}
