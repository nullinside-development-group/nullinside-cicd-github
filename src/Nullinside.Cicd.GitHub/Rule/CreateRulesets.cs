using log4net;

using Octokit;
using Octokit.GraphQL;
using Octokit.GraphQL.Core;
using Octokit.GraphQL.Model;

using Connection = Octokit.GraphQL.Connection;
using Repository = Octokit.Repository;

namespace Nullinside.Cicd.GitHub.Rule;

/// <summary>
///   Creates the branch merging rules based on the code bases' language.
/// </summary>
public class CreateRulesets : IRepoRule {
  /// <summary>
  /// The logger
  /// </summary>
  private ILog _log = LogManager.GetLogger(typeof(CreateRulesets));
  
  /// <inheritdoc />
  public async Task Handle(GitHubClient client, Connection graphQl, ID projectId, Repository repo) {
    // This currently doesn't run properly. You get an error about not specifying multiple Parameters on the status
    // checks. Waiting on an update from the source library to not include nulls in the compiled query.
    // https://github.com/octokit/octokit.graphql.net/issues/320
    IEnumerable<string>? rulesets = await graphQl.Run(new Query()
      .Repository(repo.Name, Constants.GITHUB_ORG)
      .Rulesets()
      .AllPages()
      .Select(i => i.Name));

    string? expectedRuleset =
      rulesets.FirstOrDefault(r => "main".Equals(r, StringComparison.InvariantCultureIgnoreCase));
    if (null != expectedRuleset) {
      return;
    }

    ID id = await graphQl.Run(new Query()
      .Repository(repo.Name, Constants.GITHUB_ORG)
      .Select(i => i.Id));

    _log.Info($"{repo.Name}: Creating default ruleset");
    StatusCheckConfigurationInput[]? statusChecks = null;
    if ("Typescript".Equals(repo.Language, StringComparison.InvariantCultureIgnoreCase)) {
      statusChecks = new[] {
        new StatusCheckConfigurationInput {
          Context = "ng test"
        },
        new StatusCheckConfigurationInput {
          Context = "ng lint"
        },
        new StatusCheckConfigurationInput {
          Context = "Analyze (javascript-typescript)"
        },
        new StatusCheckConfigurationInput {
          Context = "CodeQL"
        }
      };
    }
    else if ("C#".Equals(repo.Language, StringComparison.InvariantCultureIgnoreCase)) {
      statusChecks = new[] {
        new StatusCheckConfigurationInput {
          Context = "Roslyn Compiler Warnings"
        },
        new StatusCheckConfigurationInput {
          Context = "Tests"
        },
        new StatusCheckConfigurationInput {
          Context = "Analyze (csharp)"
        },
        new StatusCheckConfigurationInput {
          Context = "CodeQL"
        }
      };
    }

    var rules = new List<RepositoryRuleInput>([
      new RepositoryRuleInput {
        Type = RepositoryRuleType.Deletion
      },
      new RepositoryRuleInput {
        Type = RepositoryRuleType.PullRequest,
        Parameters = new RuleParametersInput {
          PullRequest = new PullRequestParametersInput {
            DismissStaleReviewsOnPush = true,
            RequireCodeOwnerReview = true
          }
        }
      }
    ]);

    if (null != statusChecks) {
      rules.Add(new RepositoryRuleInput {
        Type = RepositoryRuleType.RequiredStatusChecks,
        Parameters = new RuleParametersInput {
          RequiredStatusChecks = new RequiredStatusChecksParametersInput {
            RequiredStatusChecks = statusChecks,
            StrictRequiredStatusChecksPolicy = true
          }
        }
      });
    }

    await graphQl.Run(new Mutation()
      .CreateRepositoryRuleset(new Arg<CreateRepositoryRulesetInput>(new CreateRepositoryRulesetInput {
        SourceId = id,
        Name = "main",
        Enforcement = RuleEnforcement.Active,
        Target = RepositoryRulesetTarget.Branch,
        Rules = rules,
        Conditions = new RepositoryRuleConditionsInput {
          RefName = new RefNameConditionTargetInput {
            Include = new[] { "~DEFAULT_BRANCH" },
            Exclude = new string[] { }
          }
        }
      }))
      .Select(x => x.Ruleset.Id)
    );
  }
}