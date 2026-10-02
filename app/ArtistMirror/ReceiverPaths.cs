namespace ArtistMirror;

internal static class ReceiverPaths
{
    public static string? FindUxPlayExe()
    {
        foreach (var root in CandidateRoots())
        {
            var packaged = Path.Combine(root, "engine", "uxplay.exe");
            if (File.Exists(packaged))
            {
                return packaged;
            }

            var beside = Path.Combine(root, "uxplay.exe");
            if (File.Exists(beside))
            {
                return beside;
            }

            var nested = Path.Combine(root, "receiver", "uxplay", "build", "uxplay.exe");
            if (File.Exists(nested))
            {
                return nested;
            }
        }

        return null;
    }

    public static string? FindEngineBin()
    {
        var uxplay = FindUxPlayExe();
        if (uxplay is not null)
        {
            var dir = Path.GetDirectoryName(uxplay)!;
            if (File.Exists(Path.Combine(dir, "libgstreamer-1.0-0.dll"))
                || Directory.Exists(Path.Combine(dir, "gstreamer-1.0")))
            {
                return dir;
            }
        }

        return FindUcrtBin();
    }

    public static string? FindGstPluginPath()
    {
        var engine = FindEngineBin();
        if (engine is not null)
        {
            var packaged = Path.Combine(engine, "gstreamer-1.0");
            if (Directory.Exists(packaged))
            {
                return packaged;
            }
        }

        var msys = @"C:\msys64\ucrt64\lib\gstreamer-1.0";
        return Directory.Exists(msys) ? msys : null;
    }

    public static string? FindUcrtBin()
    {
        var candidates = new[]
        {
            @"C:\msys64\ucrt64\bin",
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "msys64",
                "ucrt64",
                "bin")
        };
        return candidates.FirstOrDefault(Directory.Exists);
    }

    private static IEnumerable<string> CandidateRoots()
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var starts = new List<string>();

        if (!string.IsNullOrEmpty(AppContext.BaseDirectory))
        {
            starts.Add(AppContext.BaseDirectory);
        }

        starts.Add(Environment.CurrentDirectory);

        foreach (var start in starts)
        {
            var dir = new DirectoryInfo(Path.GetFullPath(start));
            for (var i = 0; i < 8 && dir is not null; i++)
            {
                if (seen.Add(dir.FullName))
                {
                    yield return dir.FullName;
                }

                dir = dir.Parent;
            }
        }
    }
}
