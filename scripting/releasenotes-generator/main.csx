#r "nuget:Newtonsoft.Json,11.0.2"
#load "GitHubClientWrapper.csx"

using Octokit;
using Newtonsoft.Json;

var repoNames = new[]
{
    "omnisharp/omnisharp-roslyn"
};

var outputFileName = "ReleaseNotes.md";

var startDate = Args.FirstOrDefault() ?? DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");

var client = new GitHubClientWrapper();

try
{
    using (var writer = new StreamWriter(outputFileName))
    {
        writer.WriteLine($"# Changelog since {startDate}");

        foreach (var repoName in repoNames)
        {
            writer.WriteLine($"## {repoName}");

            // Pull Requests first
            Console.WriteLine($"Retrieving merged pull requests for repo {repoName}");
            var mergedPullRequests = await client.GetMergedPullRequestsForRepo(repoName, startDate);

            writer.WriteLine($"This release brings in the following new functionalities: ");
            WriteToFile(writer, mergedPullRequests);

            // Then closed issues
            Console.WriteLine($"Retrieving closed issues for repo {repoName}");
            var closedIssues = await client.GetClosedIssuesForRepo(repoName, startDate);

            writer.WriteLine("This release closes the following issues:");
            WriteToFile(writer, closedIssues);
        }
    }
    Console.WriteLine("Release notes successfully generated.");
}
catch (Exception e)
{
    Console.WriteLine("An error occured during script execution.");
    Console.WriteLine($"Error: {e.Message}");

    if (File.Exists(outputFileName))
    {
        File.Delete(outputFileName);
    }
}

void WriteToFile(TextWriter textWriter, SearchIssuesResult result)
{
    foreach (var item in result.Items)
    {
        textWriter.Write($"* {item.Title} ");

        var prefix = item.PullRequest != null ? "submitted" : "opened";
        textWriter.Write($"({prefix} by {item.User.Login}, {item.Url.ToWebsiteUrl()})");
        textWriter.Write(textWriter.NewLine);
    }

    textWriter.Write(textWriter.NewLine);
    textWriter.Flush();
}

static string ToWebsiteUrl(this string url)
    => url.Replace("https://github.com/api/v3/repos", "https://github.com");