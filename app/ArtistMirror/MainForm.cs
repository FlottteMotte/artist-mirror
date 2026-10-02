namespace ArtistMirror;

internal sealed class MainForm : Form
{
    private readonly Label _status;
    private readonly Button _primary;
    private readonly MirrorReceiver _receiver = new();
    private readonly System.Windows.Forms.Timer _watch;
    private bool _running;

    public MainForm()
    {
        Text = "artist-mirror";
        ClientSize = new Size(480, 460);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = true;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = UiTheme.BgTop;
        ForeColor = UiTheme.Ink;
        Font = new Font("Segoe UI", 10.5f, FontStyle.Regular);
        DoubleBuffered = true;

        var canvas = new MagicCanvas();

        var brand = new Label
        {
            Text = "artist-mirror",
            Font = new Font("Georgia", 30f, FontStyle.Italic),
            AutoSize = true,
            Location = new Point(36, 40),
            ForeColor = UiTheme.Ink,
            BackColor = Color.Transparent
        };

        var tag = new Label
        {
            Text = "iPad screen mirror for Windows",
            Font = new Font("Segoe UI", 10f, FontStyle.Regular),
            AutoSize = true,
            Location = new Point(42, 88),
            ForeColor = UiTheme.Accent,
            BackColor = Color.Transparent
        };

        _status = new Label
        {
            Text = "Ready",
            Font = new Font("Segoe UI Semibold", 12.5f),
            AutoSize = true,
            Location = new Point(42, 124),
            ForeColor = UiTheme.Mute,
            BackColor = Color.Transparent
        };

        var hint = new Label
        {
            Text =
                "1. Keep iPad and PC on the same Wi-Fi" + Environment.NewLine +
                "2. Tap Start" + Environment.NewLine +
                "3. On iPad: Screen Mirroring -> artist-mirror" + Environment.NewLine +
                "4. Share this window in Discord or OBS",
            Location = new Point(42, 168),
            Size = new Size(400, 140),
            ForeColor = UiTheme.Mute,
            BackColor = Color.Transparent,
            Font = new Font("Segoe UI", 11f)
        };

        _primary = new Button
        {
            Text = "Start",
            Location = new Point(42, 340),
            Size = new Size(230, 52),
            FlatStyle = FlatStyle.Flat,
            BackColor = UiTheme.Accent,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 13f),
            Cursor = Cursors.Hand,
            TabStop = true
        };
        _primary.FlatAppearance.BorderSize = 0;
        _primary.FlatAppearance.MouseOverBackColor = UiTheme.AccentHover;
        _primary.FlatAppearance.MouseDownBackColor = UiTheme.AccentPressed;
        ApplyPrimaryShape();
        _primary.Click += (_, _) => Toggle();

        canvas.Controls.Add(brand);
        canvas.Controls.Add(tag);
        canvas.Controls.Add(_status);
        canvas.Controls.Add(hint);
        canvas.Controls.Add(_primary);
        Controls.Add(canvas);

        _watch = new System.Windows.Forms.Timer { Interval = 800 };
        _watch.Tick += (_, _) => PollReceiver();
        _watch.Start();

        FormClosing += (_, _) =>
        {
            _receiver.Stop();
            _receiver.Dispose();
        };
    }

    private void Toggle()
    {
        if (_running)
        {
            StopReceiving();
        }
        else
        {
            StartReceiving();
        }
    }

    private void ApplyPrimaryShape()
    {
        _primary.Region = UiShapes.RoundedRegion(_primary.Width, _primary.Height, 26);
    }

    private void SetRunningState(bool running, string status, Color color)
    {
        _running = running;
        _status.Text = status;
        _status.ForeColor = color;
        _primary.Text = running ? "Stop" : "Start";
        _primary.BackColor = running ? UiTheme.Stop : UiTheme.Accent;
        _primary.FlatAppearance.MouseOverBackColor = running
            ? UiTheme.StopHover
            : UiTheme.AccentHover;
        ApplyPrimaryShape();
    }

    private void StartReceiving()
    {
        try
        {
            _receiver.Start();
            SetRunningState(true, "Waiting for iPad...", UiTheme.Ok);
        }
        catch (FileNotFoundException)
        {
            SetRunningState(false, "Engine missing from this folder", UiTheme.Bad);
            MessageBox.Show(
                "The mirror engine is missing.\n\n" +
                "Use the packaged artist-mirror zip\n" +
                "(the folder that includes engine).",
                "artist-mirror",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            SetRunningState(false, "Error", UiTheme.Bad);
            MessageBox.Show(ex.Message, "artist-mirror", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void StopReceiving()
    {
        _receiver.Stop();
        SetRunningState(false, "Ready", UiTheme.Mute);
    }

    private void PollReceiver()
    {
        if (!_receiver.PollExited(out var exitCode))
        {
            return;
        }

        SetRunningState(
            false,
            exitCode == 0 ? "Ready" : "Stopped - check firewall / Wi-Fi",
            exitCode == 0 ? UiTheme.Mute : UiTheme.Bad);
    }
}
