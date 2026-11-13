//using System.Net.Http.Json;
//using RestaurantSaaS.Application.Abstractions;
//using RestaurantSaaS.Application.Common.Exceptions;
//using RestaurantSaaS.Contracts.Dtos;

//namespace RestaurantSaaS.Application.Features.Licensing;

//internal sealed class HttpLicenseServerClient(LicenseOptions options) : ILicenseServerClient
//{
//    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(30) };

//    public Task<LicenseIssueDto> ActivateAsync(ServerActivateRequest request, CancellationToken ct) =>
//        PostAsync("api/v1/license-server/activate", request, ct);

//    public Task<LicenseIssueDto> StartTrialAsync(ServerTrialRequest request, CancellationToken ct) =>
//        PostAsync("api/v1/license-server/trial", request, ct);

//    private async Task<LicenseIssueDto> PostAsync(string path, object body, CancellationToken ct)
//    {
//        if (!Uri.TryCreate(options.ServerUrl, UriKind.Absolute, out var baseAddress))
//        {
//            throw new RequestRejectedException("license_server_not_set", "The License Server address is not set. Please contact support.");
//        }

//        try
//        {
//           // using var response = await Http.PostAsJsonAsync(new Uri(baseAddress, path), body, body.GetType(), ct);
//            // AB
//            using var content = JsonContent.Create(body, body.GetType());
//            using var response = await Http.PostAsync(new Uri(baseAddress, path), content, ct);
//            if (response.IsSuccessStatusCode)
//            {
//                return await response.Content.ReadFromJsonAsync<LicenseIssueDto>(ct)
//                       ?? throw new RequestRejectedException("empty_response", "The License Server returned an empty answer.");
//            }

//            ProblemPayload? problem = null;
//            try
//            {
//                problem = await response.Content.ReadFromJsonAsync<ProblemPayload>(ct);
//            }
//            catch (System.Text.Json.JsonException)
//            {
//            }

//            throw new RequestRejectedException(problem?.Type ?? "license_server_error", problem?.Title ?? $"The License Server answered with status {(int)response.StatusCode}.");
//        }
//        catch (HttpRequestException)
//        {
//            throw new RequestRejectedException("network", "The License Server could not be reached. Check the internet connection and try again.");
//        }
//        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
//        {
//            throw new RequestRejectedException("network", "The License Server did not answer in time. Please try again.");
//        }
//    }

//    private sealed record ProblemPayload(string? Type, string? Title);
//}


using System.Net.Http.Json;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Application.Common.Exceptions;
using RestaurantSaaS.Contracts.Dtos;

namespace RestaurantSaaS.Application.Features.Licensing;

internal sealed class HttpLicenseServerClient(LicenseOptions options) : ILicenseServerClient
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(30) };

    public async Task<LicenseIssueDto> ActivateAsync(ServerActivateRequest request, CancellationToken ct) =>
        await PostForIssueAsync("api/v1/license-server/activate", request, ct);

    public async Task<LicenseIssueDto> StartTrialAsync(ServerTrialRequest request, CancellationToken ct) =>
        await PostForIssueAsync("api/v1/license-server/trial", request, ct);

    public async Task DeactivateAsync(ServerDeactivateRequest request, CancellationToken ct)
    {
        using var response = await PostAsync("api/v1/license-server/deactivate", request, ct);
    }

    private async Task<LicenseIssueDto> PostForIssueAsync(string path, object body, CancellationToken ct)
    {
        using var response = await PostAsync(path, body, ct);
        return await response.Content.ReadFromJsonAsync<LicenseIssueDto>(ct)
               ?? throw new RequestRejectedException("empty_response", "The License Server returned an empty answer.");
    }

    /// Posts JSON and returns the successful response. Any failure becomes a RequestRejectedException.
    private async Task<HttpResponseMessage> PostAsync(string path, object body, CancellationToken ct)
    {
        if (!Uri.TryCreate(options.ServerUrl, UriKind.Absolute, out var baseAddress))
        {
            throw new RequestRejectedException("license_server_not_set", "The License Server address is not set. Please contact support.");
        }

        try
        {
            using var content = JsonContent.Create(body, body.GetType());
            var response = await Http.PostAsync(new Uri(baseAddress, path), content, ct);

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            using (response)
            {
                ProblemPayload? problem = null;
                try
                {
                    problem = await response.Content.ReadFromJsonAsync<ProblemPayload>(ct);
                }
                catch (System.Text.Json.JsonException)
                {
                }

                throw new RequestRejectedException(problem?.Type ?? "license_server_error", problem?.Title ?? $"The License Server answered with status {(int)response.StatusCode}.");
            }
        }
        catch (HttpRequestException)
        {
            throw new RequestRejectedException("network", "The License Server could not be reached. Check the internet connection and try again.");
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new RequestRejectedException("network", "The License Server did not answer in time. Please try again.");
        }
    }

    private sealed record ProblemPayload(string? Type, string? Title);
}
