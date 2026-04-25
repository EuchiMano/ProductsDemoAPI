using System.Text;
using Microsoft.Extensions.Logging;

namespace NewProjectFromScratch.Infrastructure.Logging
{
    public sealed class FileLoggerProvider : ILoggerProvider
    {
        private readonly StreamWriter _writer;
        private readonly object _sync = new();

        public FileLoggerProvider(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(categoryName, _writer, _sync);
        }

        public void Dispose()
        {
            _writer?.Dispose();
        }

        private sealed class FileLogger : ILogger
        {
            private readonly string _categoryName;
            private readonly StreamWriter _writer;
            private readonly object _sync;

            public FileLogger(string categoryName, StreamWriter writer, object sync)
            {
                _categoryName = categoryName;
                _writer = writer;
                _sync = sync;
            }

            public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

            public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string>? formatter)
            {
                if (formatter is null || !IsEnabled(logLevel))
                {
                    return;
                }

                var message = formatter(state, exception);
                if (string.IsNullOrEmpty(message))
                {
                    return;
                }

                var timestamp = DateTime.UtcNow.ToString("o");
                var exceptionText = exception is null ? string.Empty : Environment.NewLine + exception;
                var line = $"{timestamp} [{logLevel}] {_categoryName}: {message}{exceptionText}";

                lock (_sync)
                {
                    _writer.WriteLine(line);
                }
            }
        }

        private sealed class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new();
            public void Dispose() { }
        }
    }
}
