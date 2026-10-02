using System.Diagnostics;

namespace ArtistMirror;

internal sealed class MirrorReceiver : IDisposable
{
    public const string AirPlayName = "artist-mirror";

    private Process? _process;

    public bool IsRunning => _process is { HasExited: false };

    public int? LastExitCode { get; private set; }

    public void Start()
    {
        if (IsRunning)
        {
            return;
        }

        var uxplay = ReceiverPaths.FindUxPlayExe()
            ?? throw new FileNotFoundException("Mirror engine not found.");

        var engineDir = Path.GetDirectoryName(uxplay)!;
        var engineBin = ReceiverPaths.FindEngineBin();
        var gstPlugins = ReceiverPaths.FindGstPluginPath();

        var startInfo = new ProcessStartInfo
        {
            FileName = uxplay,
            Arguments = "-n " + AirPlayName,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = engineDir
        };

        var path = Environment.GetEnvironmentVariable("PATH") ?? "";
        if (engineBin is not null)
        {
            startInfo.Environment["PATH"] = engineBin + Path.PathSeparator + path;
        }

        if (gstPlugins is not null)
        {
            startInfo.Environment["GST_PLUGIN_PATH"] = gstPlugins;
            startInfo.Environment["GST_PLUGIN_SYSTEM_PATH"] = gstPlugins;
        }

        var registry = Path.Combine(engineDir, "gst-registry.bin");
        startInfo.Environment["GST_REGISTRY"] = registry;
        startInfo.Environment["GST_REGISTRY_1_0"] = registry;

        _process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start mirror engine.");

        _process.OutputDataReceived += (_, _) => { };
        _process.ErrorDataReceived += (_, _) => { };
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
        LastExitCode = null;
    }

    public void Stop()
    {
        try
        {
            if (_process is { HasExited: false })
            {
                _process.Kill(entireProcessTree: true);
                _process.WaitForExit(3000);
                LastExitCode = _process.ExitCode;
            }
        }
        catch
        {
        }
        finally
        {
            _process?.Dispose();
            _process = null;
        }
    }

    public bool PollExited(out int exitCode)
    {
        exitCode = 0;
        if (_process is null)
        {
            return false;
        }

        if (!_process.HasExited)
        {
            return false;
        }

        exitCode = _process.ExitCode;
        LastExitCode = exitCode;
        _process.Dispose();
        _process = null;
        return true;
    }

    public void Dispose() => Stop();
}
