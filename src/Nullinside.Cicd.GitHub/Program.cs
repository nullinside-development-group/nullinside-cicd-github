// See https://aka.ms/new-console-template for more information


/*
var github = new GithubProxy();
await github.GetAllProjects();

var repos = await github.GetAllRepos();
foreach (var repo in repos)
{
    var issues = await github.GetAllIssues(repo.name);
}*/


using Octokit.GraphQL;
using static Octokit.GraphQL.Variable;

var productInformation = new ProductHeaderValue("nullinside-cicd-github", "1.0.0");
var connection = new Connection(productInformation, Environment.GetEnvironmentVariable("GITHUB_PAT"));

var query = new Query()
    .Organization(Var("owner"))
    .ProjectsV2(
        100
    )
    .Edges
    .Select(e => new
    {
        e.Node.Id
    })
    .Compile();

/*var query = new Query()
    .RepositoryOwner(Var("owner"))
    .Repository(Var("name"))
    .Select(repo => new
    {
        repo.Id,
        repo.Name,
        repo.Owner.Login,
        repo.IsFork,
        repo.IsPrivate
    }).Compile();*/

var vars = new Dictionary<string, object>
{
    { "owner", Environment.GetEnvironmentVariable("GITHUB_ORG") },
    { "name", "nullinside-ui" }
};

var result = await connection.Run(query, vars);

foreach (var r in result) Console.WriteLine(r.Id);