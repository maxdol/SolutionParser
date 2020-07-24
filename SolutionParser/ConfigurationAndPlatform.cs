namespace SolutionParser
{
    internal class ConfigurationAndPlatform
    {
        public ConfigurationAndPlatform(string fullName)
        {
            FullName = fullName;
            int sep = FullName.IndexOf('|');
            ConfigurationName = FullName.Substring(0, sep);
            PlatformName = FullName.Substring(sep + 1);
        }

        //
        // Summary:
        //     The configuration part of this configuration - e.g. "Debug", "Release"
        public string ConfigurationName { get; }
        //
        // Summary:
        //     The platform part of this configuration - e.g. "Any CPU", "Win32"
        public string PlatformName { get; }
        //
        // Summary:
        //     The full name of this configuration - e.g. "Debug|Any CPU"
        public string FullName { get; }
    }
}
