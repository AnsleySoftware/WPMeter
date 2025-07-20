namespace WPMeter
{
    public class LogManager
    {
        private readonly string _logFilePath;

        public LogManager(string logFilePath)
        {
            if (string.IsNullOrWhiteSpace(logFilePath))
                throw new ArgumentException("logFilePath must be a non-empty path", nameof(logFilePath));
            _logFilePath = logFilePath;
        }

        public string LogFilePath => _logFilePath;

        public bool LogFileExists()
        {
            return File.Exists(_logFilePath);
        }

        public void CreateLogFile()
        {
            var directory = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var stream = File.Create(_logFilePath))
            {

            }
        }

        public void AppendLogLine(string line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));

            var directory = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.AppendAllText(_logFilePath, line + Environment.NewLine);
        }

        public IList<string> ReadAllLines()
        {
            if (!LogFileExists())
                return new List<string>();

            return File.ReadAllLines(_logFilePath);
        }

        public void DeleteLogFile()
        {
            if (LogFileExists())
            {
                File.Delete(_logFilePath);
            }
        }
    }
}
