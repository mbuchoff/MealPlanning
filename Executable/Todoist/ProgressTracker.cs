using ShellProgressBar;

namespace SystemOfEquations.Todoist;

internal sealed class ProgressTracker : IDisposable
{
    private readonly ProgressBar _progressBar;
    private readonly object _lock = new();
    private int _completedOperations;

    [ThreadStatic]
    private static ProgressTracker? _current;

    public static ProgressTracker? Current => _current;

    public ProgressTracker(int totalOperations)
    {
        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkGray,
            ProgressCharacter = '─',
            ProgressBarOnBottom = true,
            DisplayTimeInRealTime = true,
        };

        _progressBar = new ProgressBar(totalOperations, "Syncing to Todoist...", options);
        _current = this;
    }

    public void IncrementProgress(string? message = null)
    {
        lock (_lock)
        {
            _completedOperations++;
            _progressBar.Tick(message ?? string.Empty);
        }
    }

    public void UpdateMessage(string message)
    {
        lock (_lock)
        {
            _progressBar.Message = message;
        }
    }

    public void Dispose()
    {
        _progressBar.Dispose();
        if (_current == this)
        {
            _current = null;
        }
    }
}
