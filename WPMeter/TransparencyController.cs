namespace WPMeter
{
    public class TransparencyController
    {
        private readonly Form _form;
        private readonly SettingsManager _settings;
        public ToolStripMenuItem? MenuItem { get; }

        public TransparencyController(Form form, SettingsManager settings)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            var slider = new TrackBar
            {
                Minimum = 20,
                Maximum = 100,
                TickFrequency = 10,
                Value = (int)(_form.Opacity * 100),
                TickStyle = TickStyle.None,
                AutoSize = false,
                Width = 120,
                Height = 20
            };
            slider.ValueChanged += (s, e) =>
            {
                _form.Opacity = slider.Value / 100.0;
                _settings.Opacity = slider.Value / 100.0;
            };

            var host = new CompactControlHost(slider) { Size = slider.Size };
            MenuItem = new ToolStripMenuItem("Transparency");
            MenuItem.DropDown.Padding = Padding.Empty;
            MenuItem.DropDown.Items.Add(host);
        }
    }
}
