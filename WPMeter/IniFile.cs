using System.Runtime.InteropServices;
using System.Text;

namespace WPMeter
{
    public static class IniFile
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern long GetPrivateProfileString(
            string section, string key, string defaultValue,
            StringBuilder retVal, int size, string filePath);

        public static void Write(string section, string key, string value, string filePath)
        {
            WritePrivateProfileString(section, key, value, filePath);
        }

        public static string Read(string section, string key, string defaultValue, string filePath)
        {
            var sb = new StringBuilder(256);
            GetPrivateProfileString(section, key, defaultValue, sb, sb.Capacity, filePath);
            return sb.ToString();
        }
    }
}
