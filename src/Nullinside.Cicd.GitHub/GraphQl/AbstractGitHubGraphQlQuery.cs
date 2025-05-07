using System.Net.Http.Headers;
using System.Text;

using Newtonsoft.Json;

using Nullinside.Cicd.GitHub.GraphQl.Model;

namespace Nullinside.Cicd.GitHub.GraphQl;

/// <summary>
///   The contract for executing a simple graphql query.
/// </summary>
/// <typeparam name="T">The JSON response POCO to the query, <see cref="GraphQlGenericMutationResponse" /> for no response.</typeparam>
public abstract class AbstractGitHubGraphQlQuery<T> {
  /// <summary>
  ///   The query, typically coded into the object.
  /// </summary>
  public abstract string Query { get; }

  /// <summary>
  ///   The query variables, typically assembled in the constructor.
  /// </summary>
  public object? QueryVariables { get; set; }

  /// <summary>
  ///   Executes the request.
  /// </summary>
  /// <returns>The response object.</returns>
  public virtual async Task<T?> SendAsync() {
    using var httpClient = new HttpClient {
      BaseAddress = new Uri(Constants.GITHUB_GRAPHQL_URL),
      DefaultRequestHeaders = {
        UserAgent = { new ProductInfoHeaderValue(Constants.PROJECT_NAME, "0.0.0") },
        Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("GITHUB_PAT"))
      }
    };

    var queryObject = new {
      query = Query,
      variables = QueryVariables
    };

    var request = new HttpRequestMessage {
      Method = HttpMethod.Post,
      Content = new StringContent(JsonConvert.SerializeObject(queryObject), Encoding.UTF8, "application/json")
    };

    using HttpResponseMessage response = await httpClient.SendAsync(request);
    response.EnsureSuccessStatusCode();
    string responseString = await response.Content.ReadAsStringAsync();

    return JsonConvert.DeserializeObject<T>(responseString);
  }
}