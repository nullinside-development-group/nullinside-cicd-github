using Nullinside.Cicd.GitHub.GraphQl.Model;

namespace Nullinside.Cicd.GitHub.GraphQl;

/// <summary>
///   Changes the status of the issue on the GitHub project board. (ex: Backlog, Ready, In progress, In review, Done)
/// </summary>
public class MutateIssueProjectStatuses : AbstractGitHubGraphQlQuery<GraphQlGenericMutationResponse> {
  /// <summary>
  ///   Initializes a new instance of the <see cref="MutateIssueProjectStatuses" /> class.
  /// </summary>
  /// <param name="projectId">The project id</param>
  /// <param name="projectIssueId">The status field's id (ex: PVTI_lADOCZOBm84AdCw7zgQfohA NOT I_kwDOLcGb986MVe7p)</param>
  /// <param name="statusFieldId">The status field's id (ex: PVTSSF_lADOCZOBm84AdCw7zgSzu_A)</param>
  /// <param name="selectionId">The unique identifier of the single selection option to change the status to.</param>
  public MutateIssueProjectStatuses(string projectId, string projectIssueId, string statusFieldId, string selectionId) {
    QueryVariables = new { project = projectId, item = projectIssueId, field = statusFieldId, selectionId };
  }

  /// <inheritdoc />
  public override string Query => @"mutation($project: ID!, $item: ID!, $field: ID!, $selectionId: String!) {
    updateProjectV2ItemFieldValue(
      input: {projectId: $project, itemId: $item, fieldId: $field, value: {singleSelectOptionId: $selectionId}}
    ) {
      clientMutationId
    }
  }";
}