namespace WPMeter
{
    public class TypingController : IDisposable
    {
        private readonly WPMCalculator _calculator = new WPMCalculator();
        private readonly TypingSnapshotManager? _snapshotManager;
        private readonly HashSet<Keys> _heldKeys = new HashSet<Keys>();
        private bool _loggingEnabled = false;

        public event Action<double?>? WpmUpdated;

        public TypingController(LogManager logManager)
        {
            _snapshotManager = new TypingSnapshotManager(
                interval: TimeSpan.FromSeconds(10),
                minKeysPerSlice: 3,
                minSliceDuration: TimeSpan.FromSeconds(3),
                logManager: logManager);
        }

        public void EnableLogging()
        {
            _loggingEnabled = true;
            _snapshotManager!.Start();
        }

        public void DisableLogging()
        {
            _loggingEnabled = false;
            _snapshotManager!.Stop();
            _snapshotManager.Reset();
        }

        public void HandleKeyDown(Keys key)
        {
            if (_heldKeys.Add(key))
            {
                _calculator.HandleKeyPress(key);
                if (_loggingEnabled)
                    _snapshotManager!.HandleKeyDown(key);

                WpmUpdated?.Invoke(_calculator.GetCurrentWpm());
            }
        }

        public void HandleKeyUp(Keys key)
        {
            _heldKeys.Remove(key);
            if (_loggingEnabled)
                _snapshotManager!.HandleKeyUp(key);
        }

        public void Dispose()
        {
            _snapshotManager!.Dispose();
        }
    }
}
