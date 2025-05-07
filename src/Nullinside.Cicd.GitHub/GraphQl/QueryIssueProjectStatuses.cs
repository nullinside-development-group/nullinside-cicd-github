using Nullinside.Cicd.GitHub.Model;

namespace Nullinside.Cicd.GitHub.GraphQl;

/// <summary>
///   Queries the issues in a GitHub code base.
/// </summary>
public class QueryIssueProjectStatuses : AbstractGitHubGraphQlQuery<GraphQlProjectIssueResponse> {
  /// <summary>
  ///   Initializes a new instance of the <see cref="QueryIssueProjectStatuses" /> class.
  /// </summary>
  /// <param name="repoOwner">The owner of the repo on github.</param>
  /// <param name="repoName">The name of the repo on github.</param>
  public QueryIssueProjectStatuses(string repoOwner, string repoName) {
    QueryVariables = new { owner = repoOwner, name = repoName };
  }

  /// <inheritdoc />
  public override string Query => @"query ($name: String!, $owner: String!) {
        repository(name: $name, owner: $owner) {
          id
          issues(first: 100) {
            pageInfo {
              hasNextPage
              endCursor
            }
            nodes {
              id
              number
              closed
              projectItems(first: 100) {
                pageInfo {
                  hasNextPage
                  endCursor
                }
                nodes {
                  id
                  fieldValueByName(name: ""Status"") {
                    ... on ProjectV2ItemFieldSingleSelectValue {
                      name
                      field {
                        ... on ProjectV2SingleSelectField {
                         id
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }";
}