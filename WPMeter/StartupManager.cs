using Microsoft.Win32;

namespace WPMeter
{
    public class StartupManager
    {
        private readonly string? _appName;
        private readonly string? _appPath;

        public StartupManager(string appName, string appPath)
        {
            _appName = appName;
            _appPath = appPath;
        }

        public bool IsEnabled()
        {
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false))
            {
                return key?.GetValue(_appName)?.ToString() == _appPath;
            }
        }

        public void SetEnabled(bool enable)
        {
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                if (key == null) return;

                if (enable)
                    key.SetValue(_appName, _appPath!);
                else
                    key.DeleteValue(_appName!, false);
            }
        }
    }
}
