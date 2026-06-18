using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ChronoSync.Configuration;
using ChronoSync.Models;
using Microsoft.Extensions.Options;

namespace ChronoSync.Clients;

public class LumaClient : ILumaClient
{
    private readonly HttpClient _httpClient;
    private readonly LumaSettings _settings;

    private string? _accessToken;

    private readonly JsonSerializerOptions _jsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true
        };

    public LumaClient(
        HttpClient httpClient,
        IOptions<AppSettings> options)
    {
        _httpClient = httpClient;
        _settings = options.Value.Luma;

        _httpClient.BaseAddress =
            new Uri(_settings.BaseUrl);
    }

    public async Task<StartResponse> StartAsync()
    {
        var route =
            _settings.Level == 2
                ? "/api/start/2"
                : "/api/start";

        var request = new StartRequest
        {
            Username = _settings.Username,
            Email = _settings.Email
        };

        var response =
            await _httpClient.PostAsync(
                route,
                CreateJsonContent(request));

        var result =
            await DeserializeAsync<StartResponse>(response);

        if (!string.IsNullOrWhiteSpace(result.AccessToken))
        {
            _accessToken = result.AccessToken;
        }

        return result;
    }

    public async Task<ProbeResponse> GetProbesAsync()
    {
        var response =
            await SendAuthenticatedAsync(
                HttpMethod.Get,
                "/api/probe");

        return await DeserializeAsync<ProbeResponse>(response);
    }

    public async Task<SyncResponse> SyncProbeAsync(
        string probeId)
    {
        var response =
            await SendAuthenticatedAsync(
                HttpMethod.Post,
                $"/api/probe/{probeId}/sync");

        return await DeserializeAsync<SyncResponse>(response);
    }

    public async Task<TakeJobResponse> TakeJobAsync()
    {
        var response =
            await SendAuthenticatedAsync(
                HttpMethod.Post,
                "/api/job/take");

        return await DeserializeAsync<TakeJobResponse>(
            response);
    }

    public async Task<CheckJobResponse> CheckJobAsync(
        string jobId,
        CheckJobRequest request)
    {
        var response =
            await SendAuthenticatedAsync(
                HttpMethod.Post,
                $"/api/job/{jobId}/check",
                CreateJsonContent(request));

        return await DeserializeAsync<CheckJobResponse>(
            response);
    }

    private async Task<HttpResponseMessage> SendAuthenticatedAsync(
        HttpMethod method,
        string route,
        HttpContent? content = null)
    {
        if (string.IsNullOrWhiteSpace(_accessToken))
        {
            throw new InvalidOperationException(
                "Authentication token is missing. Call StartAsync first.");
        }

        using var request = new HttpRequestMessage(method, route)
        {
            Content = content
        };

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _accessToken);

        return await _httpClient.SendAsync(request);
    }

    private StringContent CreateJsonContent<T>(
        T value)
    {
        return new StringContent(
            JsonSerializer.Serialize(value),
            Encoding.UTF8,
            "application/json");
    }

    private async Task<T> DeserializeAsync<T>(
        HttpResponseMessage response)
    {
        var json =
            await response.Content.ReadAsStringAsync();

        var result =
            JsonSerializer.Deserialize<T>(
                json,
                _jsonOptions);

        if (result == null)
        {
            throw new Exception(
                $"Failed to deserialize {typeof(T).Name}");
        }

        return result;
    }
}