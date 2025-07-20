namespace WPMeter
{
    public class TextBoxInputSource : IInputSource
    {
        private readonly TextBox? _textBox;

        public event EventHandler<KeyEventArgs>? KeyPressed;
        public event EventHandler<KeyEventArgs>? KeyReleased;

        public TextBoxInputSource(TextBox textBox)
        {
            _textBox = textBox;
        }

        public void Start()
        {
            _textBox!.KeyDown += TextBox_KeyDown;
            _textBox.KeyUp += TextBox_KeyUp;
            _textBox.Visible = true;
            _textBox.Focus();
        }

        private void TextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            KeyPressed?.Invoke(this, e);
        }

        private void TextBox_KeyUp(object? sender, KeyEventArgs e)
        {
            KeyReleased?.Invoke(this, e);
        }

        public void Stop()
        {
            _textBox!.KeyDown -= TextBox_KeyDown;
            _textBox.KeyUp -= TextBox_KeyUp;
            _textBox.Visible = false;
        }

        public void Dispose() => Stop();
    }
}
