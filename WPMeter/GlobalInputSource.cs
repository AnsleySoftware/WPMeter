using Gma.System.MouseKeyHook;

namespace WPMeter
{
    public class GlobalInputSource : IInputSource
    {
        private IKeyboardMouseEvents? _hook;

        public event EventHandler<KeyEventArgs>? KeyPressed;
        public event EventHandler<KeyEventArgs>? KeyReleased;

        public void Start()
        {
            _hook = Hook.GlobalEvents();
            _hook.KeyDown += Hook_KeyDown;
            _hook.KeyUp += Hook_KeyUp;
        }

        private void Hook_KeyDown(object? sender, KeyEventArgs e)
        {
            KeyPressed?.Invoke(this, e);
        }

        private void Hook_KeyUp(object? sender, KeyEventArgs e)
        {
            KeyReleased?.Invoke(this, e);
        }

        public void Stop()
        {
            if (_hook != null)
            {
                _hook.KeyDown -= Hook_KeyDown;
                _hook.KeyUp -= Hook_KeyUp;
                _hook.Dispose();
                _hook = null;
            }
        }

        public void Dispose() => Stop();
    }
}