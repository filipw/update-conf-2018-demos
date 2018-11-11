#r "nuget:Octokit, 0.29.0"

using Octokit;
using Octokit.Internal;

public class GitHubClientWrapper
{
    private readonly GitHubClient _client;

    public GitHubClientWrapper()
    {
        var accessToken = Environment.GetEnvironmentVariable("GITHUBTOKEN");

        if (string.IsNullOrWhiteSpace(accessToken)) throw new Exception("Could not fetch the required access token to acces the GitHub repository.");
        
        var githubUri = new Uri("https://github.com/");
        var credentials = new Credentials(accessToken);
        _client = new GitHubClient(new ProductHeaderValue("DemoGitHubClient"), githubUri);
        _client.Credentials = credentials;
    }

    public Task<SearchIssuesResult> GetMergedPullRequestsForRepo(string repoName, string startDate)
    {
        var parsedStartDate = DateTime.Parse(startDate);
        var req = new SearchIssuesRequest();
        req.Repos.Add(repoName);
        req.Type = IssueTypeQualifier.PullRequest;
        req.State = ItemState.Closed;
        req.Merged = new DateRange(parsedStartDate, SearchQualifierOperator.GreaterThanOrEqualTo);
        return _client.Search.SearchIssues(req);
    }

    public Task<SearchIssuesResult> GetClosedIssuesForRepo(string repoName, string startDate)
    {
        var parsedStartDate = DateTime.Parse(startDate);
        var req = new SearchIssuesRequest();
        req.Repos.Add(repoName);
        req.Type = IssueTypeQualifier.Issue;
        req.State = ItemState.Closed;
        req.Closed = new DateRange(parsedStartDate, SearchQualifierOperator.GreaterThanOrEqualTo);
        return _client.Search.SearchIssues(req);
    }
}

