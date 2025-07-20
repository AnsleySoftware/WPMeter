namespace WPMeter
{
    public class UIController
    {
        private readonly Form _form;
        private readonly SettingsManager _settings;
        private readonly Label _wpmLabel;
        private readonly Panel _resizeGrip;

        private Point _resizeStart;
        private Size _originalSize;
        private bool _resizing;

        public UIController(Form form, SettingsManager settings)
        {
            _form = form ?? throw new ArgumentNullException("formUIController");
            _settings = settings ?? throw new ArgumentNullException("settingsUIController");
            _wpmLabel = CreateWpmLabel();
            _resizeGrip = CreateResizeGrip();

            
            _form.Controls.Add(_resizeGrip);
            _form.Controls.Add(_wpmLabel);

            _form.LocationChanged += (s, e) =>
            {
                _settings.WindowLocation = _form.Location;
            };
            
        }

        public Label WpmLabel => _wpmLabel;
        public Panel ResizeGrip => _resizeGrip;

        private Label CreateWpmLabel()
        {
            var label = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "--WPM"
            };

            label.MouseDown += (_, e) => DragForm(e);
            return label;
        }

        private Panel CreateResizeGrip()
        {
            var grip = new Panel
            {
                Size = new Size(12, 12),
                BackColor = Color.DarkGray,
                Cursor = Cursors.SizeNWSE,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };

            grip.MouseDown += Grip_MouseDown;
            grip.MouseMove += Grip_MouseMove;
            grip.MouseUp += Grip_MouseUp;
            return grip;
        }

        private void Grip_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _resizing = true;
                _resizeStart = Cursor.Position;
                _originalSize = _form.Size;
            }
        }

        private void Grip_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_resizing) return;

            var current = Cursor.Position;
            int dx = current.X - _resizeStart.X;
            int dy = current.Y - _resizeStart.Y;

            _form.Size = new Size(
                Math.Max(100, _originalSize.Width + dx),
                Math.Max(100, _originalSize.Height + dy));
        }

        private void Grip_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _resizing = false;
                _settings.WindowSize = _form.Size;
                _settings.WindowLocation = _form.Location;
            }
        }

        private void DragForm(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(_form.Handle, 0xA1, 0x2, 0);
            }
        }
    }
}
