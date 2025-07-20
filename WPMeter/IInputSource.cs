namespace WPMeter
{
    public interface IInputSource : IDisposable
    {
        event EventHandler<KeyEventArgs> KeyPressed;
        event EventHandler<KeyEventArgs> KeyReleased;
        void Start();
        void Stop();
    }
}
