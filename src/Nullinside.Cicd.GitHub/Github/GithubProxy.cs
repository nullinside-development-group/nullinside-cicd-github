using System.Net.Http.Headers;
using Newtonsoft.Json;
using Nullinside.Cicd.GitHub.Github.Model;

namespace Nullinside.Cicd.GitHub.Github;

public class GithubProxy
{
    private readonly string? _org = Environment.GetEnvironmentVariable("GITHUB_ORG");
    private readonly string? _token = Environment.GetEnvironmentVariable("GITHUB_PAT");
    private readonly string? _tokenClassic = Environment.GetEnvironmentVariable("GITHUB_PAT_CLASSIC");

    public async Task<IEnumerable<ReposResponse>> GetAllRepos(string? org = null)
    {
        org ??= _org;

        var client = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/orgs/{org}/repos");
        SetHeaders(request);

        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<List<ReposResponse>>(content) ?? Enumerable.Empty<ReposResponse>();
    }

    public async Task<IEnumerable<IssuesResponse>> GetAllIssues(string repoName, string? org = null)
    {
        org ??= _org;

        var client = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/repos/{org}/{repoName}/issues");
        SetHeaders(request);

        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<List<IssuesResponse>>(content) ?? Enumerable.Empty<IssuesResponse>();
    }

    public async Task<IEnumerable<IssuesResponse>> GetAllProjects(string? org = null)
    {
        org ??= _org;

        var client = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/orgs/{org}/projects");
        SetHeaders(request);

        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<List<IssuesResponse>>(content) ?? Enumerable.Empty<IssuesResponse>();
    }

    private void SetHeaders(HttpRequestMessage request)
    {
        request.Headers.Add("User-Agent", "nullinside-cicd-github");
        request.Headers.Add("Accept", "application/vnd.github+json");
        request.Headers.Add("Authorization", $"Bearer {_token}");
        request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
    }
}