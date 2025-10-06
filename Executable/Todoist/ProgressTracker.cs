using ShellProgressBar;

namespace SystemOfEquations.Todoist;

internal sealed class ProgressTracker : IDisposable
{
    private readonly ProgressBar _progressBar;
    private readonly object _lock = new();
    private int _completedOperations;

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
    }

    public void IncrementProgress(string? message = null)
    {
        lock (_lock)
        {
            _completedOperations++;
            _progressBar.Tick(message ?? string.Empty);
        }
    }

    public void Dispose()
    {
        _progressBar.Dispose();
    }
}
