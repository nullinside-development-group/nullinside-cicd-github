using Octokit;
using Octokit.GraphQL;

using Connection = Octokit.GraphQL.Connection;

namespace Nullinside.Cicd.GitHub.Rule;

public interface IRepoRule {
  Task Handle(GitHubClient client, Connection graphQl, ID projectId, Repository repo);
}