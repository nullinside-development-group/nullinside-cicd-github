using Octokit;
using Octokit.GraphQL;
using Octokit.GraphQL.Core;
using Octokit.GraphQL.Model;

using Connection = Octokit.GraphQL.Connection;
using Repository = Octokit.Repository;

namespace Nullinside.Cicd.GitHub.Rule;

/// <summary>
///   Handles associating issues with the project automatically.
/// </summary>
public class AssociateIssuesWithProject : IRepoRule {
  /// <inheritdoc />
  public async Task Handle(GitHubClient client, Connection graphQl, ID projectId, Repository repo) {
    if (!repo.HasIssues) {
      return;
    }

    var issues = await graphQl.Run(new Query()
      .Repository(repo.Name, Constants.GITHUB_ORG)
      .Issues()
      .AllPages()
      .Select(i => new {
        i.Id,
        i.Number,
        ProjectItems = i.ProjectItems(null, null, null, null, null)
          .AllPages()
          .Select(p => p.Id)
          .ToList()
      }));

    foreach (var issue in issues) {
      if (issue.ProjectItems.Count > 0) {
        continue;
      }

      Console.WriteLine($"{repo.Name}: Associating issue #{issue.Number}");
      await graphQl.Run(new Mutation()
        .AddProjectV2ItemById(new Arg<AddProjectV2ItemByIdInput>(new AddProjectV2ItemByIdInput {
          ProjectId = projectId,
          ContentId = issue.Id
        }))
        .Select(x => x.Item.Id));
    }
  }
}