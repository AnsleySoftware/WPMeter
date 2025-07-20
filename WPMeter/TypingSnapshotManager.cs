namespace WPMeter
{
    public class TypingSnapshotManager
    {
        private DateTime? _sliceStart;
        private DateTime? _sliceEnd;
        private readonly TimeSpan _interval;
        private readonly TimeSpan _minSliceDuration;
        private readonly int _minKeysPerSlice;
        private readonly LogManager? _logManager;
        private readonly System.Windows.Forms.Timer? _timer;
        private readonly HashSet<Keys> _heldKeys = new HashSet<Keys>();
        private int _keysThisSlice = 0;
        private int _totalKeys = 0;
        private double _totalMinutes = 0;

        public TypingSnapshotManager(TimeSpan interval, int minKeysPerSlice, TimeSpan minSliceDuration, LogManager logManager)
        {
            _interval = interval;
            _minKeysPerSlice = minKeysPerSlice;
            _minSliceDuration = minSliceDuration;
            _logManager = logManager;

            _timer = new System.Windows.Forms.Timer { Interval = (int)interval.TotalMilliseconds };

            _timer.Tick += OnSnapshot;
        }
        public void Start() => _timer!.Start();
        public void Stop() => _timer!.Stop();

        public void HandleKeyDown(Keys key)
        {
            if (_heldKeys.Add(key))
            {
                if (_sliceStart == null)
                    _sliceStart = DateTime.Now;

                _sliceEnd = DateTime.Now;
                _keysThisSlice++;
            }
                
        }

        public void Reset()
        {
            _timer!.Stop();
            _keysThisSlice = 0;
            _sliceStart = _sliceEnd = null;
        }

        public void HandleKeyUp(Keys key)
        {
            _heldKeys.Remove(key);
        }

        private void OnSnapshot(object? sender, EventArgs e)
        {
            var span = _sliceEnd - _sliceStart;
            if (_keysThisSlice < _minKeysPerSlice
                || span == null
                || span.Value < _minSliceDuration)
            {
                _keysThisSlice = 0;
                _sliceStart = _sliceEnd = null;
                return;
            }

            double sliceMin = Math.Min(span.Value.TotalMinutes, _interval.TotalMinutes);

            _totalKeys += _keysThisSlice;
            _totalMinutes += sliceMin;
            double avgWpm = (_totalKeys / 5.0) / _totalMinutes;

            var now = DateTime.Now;
            var line = $"{now:yyyy-MM-dd HH:mm:ss}\t" +
                $"sliceKeys={_keysThisSlice}\t" +
                $"sliceMin={sliceMin:F4}\t" +
                $"totKeys={_totalKeys}\t" +
                $"totMin={_totalMinutes:F4}\t" +
                $"avgWpm={avgWpm:F2}";
            _logManager!.AppendLogLine(line);

            _keysThisSlice = 0;
            _sliceStart = _sliceEnd = null;
        }

        public void Dispose()
        {
            _timer!.Tick -= OnSnapshot;
            _timer.Stop();
            _timer.Dispose();
        }

       
    }
}
