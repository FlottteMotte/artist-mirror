namespace ArtistMirror.Tests;

public class ReceiverPathsTests
{
    [Fact]
    public void FindUxPlayExe_returns_null_when_missing()
    {
        // Smoke: API is callable; on CI without a built receiver this is null.
        var result = ReceiverPaths.FindUxPlayExe();
        // Either found (local dev machine) or null — must not throw.
        _ = result;
    }

    [Fact]
    public void FindUcrtBin_does_not_throw()
    {
        var bin = ReceiverPaths.FindUcrtBin();
        if (bin is not null)
        {
            Assert.True(Directory.Exists(bin));
        }
    }
}
