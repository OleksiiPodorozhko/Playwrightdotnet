namespace PlaywrightDemo.Artifacts;

public static class ArtifactNameHelper
{
    public static string RepositoryRoot { get; } = ResolveRepositoryRoot();

    public static string SafeTestName(string testName)
    {
        var lastSegment = testName.Split('.').LastOrDefault() ?? testName;
        var invalidChars = Path.GetInvalidFileNameChars();
        var safeChars = lastSegment
            .Select(character => invalidChars.Contains(character) ? '_' : character)
            .ToArray();

        var safeName = new string(safeChars).Trim('.', ' ');

        if (string.IsNullOrWhiteSpace(safeName))
        {
            return "unnamed";
        }

        return safeName.Length <= 120 ? safeName : safeName[..120];
    }

    private static string ResolveRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
    }
}
