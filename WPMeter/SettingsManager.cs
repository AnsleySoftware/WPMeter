namespace WPMeter
{
    public class SettingsManager
    {
        private readonly string _iniPath;

        public SettingsManager(string iniPath)
        {
            _iniPath = iniPath;
        }

        public bool ShowReadmeOnLaunch
        {
            get => bool.TryParse(IniFile.Read("General", "ShowReadMe", "true", _iniPath), out var result) && result;
            set => IniFile.Write("General", "ShowReadme", value.ToString().ToLower(), _iniPath);
        }

        public bool KeepLogs
        {
            get => bool.TryParse(IniFile.Read("General", "KeepLogs", "false", _iniPath), out var result) && result;
            set => IniFile.Write("General", "KeepLogs", value.ToString().ToLower(), _iniPath);
        }

        public Point WindowLocation
        {
            get
            {
                var x = int.TryParse(IniFile.Read("Window", "X", "20", _iniPath), out var xVal) ? xVal : 20;
                var y = int.TryParse(IniFile.Read("Window", "Y", "20", _iniPath), out var yVal) ? yVal : 20;
                return new Point(x, y);
            }
            set
            {
                IniFile.Write("Window", "X", value.X.ToString(), _iniPath);
                IniFile.Write("Window", "Y", value.Y.ToString(), _iniPath);
            }
        }

        public Size WindowSize
        {
            get
            {
                var wStr = IniFile.Read("Window", "Width", "150", _iniPath);
                var hStr = IniFile.Read("Window", "Height", "90", _iniPath);
                if (int.TryParse(wStr, out var w) && int.TryParse(hStr, out var h))
                    return new Size (w, h);
                return new Size(150, 90);
            }
            set
            {
                IniFile.Write("Window", "Width", value.Width.ToString(), _iniPath);
                IniFile.Write("Window", "Height", value.Height.ToString(), _iniPath);
            }
        }

        public Color BackColor
        {
            get
            {
                var colHex = IniFile.Read("Appearance", "Color", Color.Black.ToArgb().ToString("X8"), _iniPath);
                if (int.TryParse(colHex, System.Globalization.NumberStyles.HexNumber, null, out var argb)) 
                    return Color.FromArgb(argb);
                return Color.Black;
            }
            set
            {
                IniFile.Write("Appearance", "Color", value.ToArgb().ToString("X8"), _iniPath);
            }
        }

        public double Opacity
        {
            get
            {
                var opStr = IniFile.Read("Appearance", "Opacity", "60", _iniPath);
                if (int.TryParse(opStr, out var opVal))
                    return Math.Clamp(opVal / 100.0, 0.2, 1.0);
                return 0.6;
            }
            set
            {
                var percent = (int)(Math.Clamp(value, 0.2, 1.0) * 100);
                IniFile.Write("Appearance", "Opacity", percent.ToString(), _iniPath);
            }
        }

        public bool UseGlobalHook
        {
            get
            {
                var hookStr = IniFile.Read("Behavior", "GlobalHook", "false", _iniPath);
                return bool.TryParse(hookStr, out var useGlobal) && useGlobal;
            }
            set
            {
                IniFile.Write("Behavior", "GlobalHook", value.ToString(), _iniPath);
            }
        }
    }
}
