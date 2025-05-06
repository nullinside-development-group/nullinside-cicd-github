using Octokit;
using Octokit.GraphQL;

using Connection = Octokit.GraphQL.Connection;

namespace Nullinside.Cicd.GitHub.Rule;

/// <summary>
///   Contracts for executing code against a project.
/// </summary>
public interface IRepoRule {
  /// <summary>
  ///   Handles performing the rule updates.
  /// </summary>
  /// <param name="client">The github rest api client.</param>
  /// <param name="graphQl">The github graphql client.</param>
  /// <param name="projectId">The unique identifier of the project.</param>
  /// <param name="repo">The git repo being updated.</param>
  /// <returns></returns>
  Task Handle(GitHubClient client, Connection graphQl, ID projectId, Repository repo);
}