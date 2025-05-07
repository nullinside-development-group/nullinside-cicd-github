using log4net;

using Nullinside.Cicd.GitHub.GraphQl;
using Nullinside.Cicd.GitHub.Model;

using Octokit;
using Octokit.GraphQL;

using Connection = Octokit.GraphQL.Connection;
using Repository = Octokit.Repository;

namespace Nullinside.Cicd.GitHub.Rule;

/// <summary>
///   Handles moving issues to done status.
/// </summary>
public class MoveClosedToDone : IRepoRule {
  /// <summary>
  ///   The logger
  /// </summary>
  private readonly ILog _log = LogManager.GetLogger(typeof(MoveClosedToDone));

  /// <inheritdoc />
  public async Task Handle(GitHubClient client, Connection graphQl, ID projectId, Repository repo) {
    if (!repo.HasIssues) {
      return;
    }

    var query = new QueryIssueProjectStatuses(Constants.GITHUB_ORG, repo.Name);
    GraphQlProjectIssueResponse? issueInfoResponse = await query.SendAsync();

    var issueStatuses = from issueInfo in issueInfoResponse?.Data.Repository.Issues.Nodes
      from projects in issueInfo.ProjectItems.Nodes
      select new { IssueId = issueInfo.Id, IssueNumber = issueInfo.Number, IsClosed = issueInfo.Closed, ProjectIssueId = projects.Id, FieldId = projects.FieldValueByName.Field.Id, Status = projects.FieldValueByName.Name };

    foreach (var issue in issueStatuses) {
      if (!issue.IsClosed || "Done".Equals(issue.Status, StringComparison.InvariantCultureIgnoreCase)) {
        continue;
      }

      _log.Info($"{repo.Name}: Moving issue to done column (issue #{issue.IssueNumber})");

      var mutation = new MutateIssueProjectStatuses(projectId.Value, issue.ProjectIssueId, issue.FieldId, Constants.GITHUB_PROJECT_DONE_SINGLE_SELECT_ID);
      await mutation.SendAsync();
    }
  }
}