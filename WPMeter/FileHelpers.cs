namespace WPMeter
{
    public static class FileHelpers
    {
        public static string EnsureAppDataDirectory(string appName)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = Path.Combine(appData, appName);
            Directory.CreateDirectory(folder);
            return folder;
        }

        public static void EnsureFileExists(string path)
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "");
            }
        }
    }
}
