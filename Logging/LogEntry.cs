using System;
using Microsoft.Extensions.Logging;

namespace apiauthz.Logging
{
    public class LogEntry
    {
        private readonly ILogger<LogEntry> _logger;

        public LogEntry (ILogger<LogEntry> logger)
        {
            _logger = logger;
        }

        public Guid TransactionId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ElapsedTimeInMillis { get; set; }
        public Guid CorrelationId { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }

        public LogEntry initLogEntry ()
        {
            LogEntry logEntry = new LogEntry(_logger);
            logEntry.TransactionId = new Guid();
            logEntry.StartTime = DateTime.UtcNow;
            return logEntry;
        }

        public void finalizeLogEntry(LogEntry logEntry)
        {

            logEntry.EndTime = DateTime.UtcNow;
        }
    }
}
