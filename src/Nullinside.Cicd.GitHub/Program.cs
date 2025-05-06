// See https://aka.ms/new-console-template for more information

using Nullinside.Cicd.GitHub;
using Nullinside.Cicd.GitHub.Rule;

using Octokit;
using Octokit.GraphQL;

using Connection = Octokit.GraphQL.Connection;
using ID = Octokit.GraphQL.ID;
using ProductHeaderValue = Octokit.ProductHeaderValue;
using Query = Octokit.GraphQL.Query;

IRepoRule?[] rules = AppDomain.CurrentDomain.GetAssemblies()
  .SelectMany(a => a.GetTypes())
  .Where(t => typeof(IRepoRule).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
  .Select(t => Activator.CreateInstance(t) as IRepoRule)
  .Where(o => null != o)
  .ToArray();

while (true) {
  var client = new GitHubClient(new ProductHeaderValue("nullinside")) {
    Credentials = new Credentials(Environment.GetEnvironmentVariable("GITHUB_PAT"))
  };

  var graphQl = new Connection(new Octokit.GraphQL.ProductHeaderValue("nullinside"),
    Environment.GetEnvironmentVariable("GITHUB_PAT"));

  ID projectId = await graphQl.Run(new Query()
    .Organization(Constants.GITHUB_ORG)
    .ProjectV2(Constants.GITHUB_PROJECT_NUM)
    .Select(p => p.Id));

  IReadOnlyList<Repository>? repository = await client.Repository.GetAllForOrg(Constants.GITHUB_ORG);
  foreach (Repository repo in repository) {
    if (repo.Private) {
      continue;
    }

    foreach (IRepoRule? rule in rules) {
      await rule!.Handle(client, graphQl, projectId, repo);
    }
  }

  Console.WriteLine("Waiting for next execution time...");
  Task.WaitAll(Task.Delay(TimeSpan.FromMinutes(5)));
}