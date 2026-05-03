using System.Globalization;
using System.Reflection;
using System.Resources;

namespace WTGUtility.Localization
{
    /// <summary>
    /// Provides localized strings for the application.
    /// Automatically follows CurrentUICulture; can be overridden via --lang.
    /// </summary>
    public static class Loc
    {
        private static readonly ResourceManager _resources =
            new ResourceManager("WTGUtility.Localization.Strings", typeof(Loc).Assembly);

        private static readonly ResourceManager _resourcesZhCN =
            /// For zh-CN, we embed a separate resource to avoid creating a satellite assembly, ensuring it works even if the satellite DLL is missing.
            new ResourceManager("WTGUtility.Localization.StringsZhCN", typeof(Loc).Assembly);

        private static CultureInfo _culture = CultureInfo.CurrentUICulture;

        private static string _version;

        /// <summary>Current UI culture for localization.</summary>
        public static CultureInfo Culture
        {
            get => _culture;
            set
            {
                _culture = value ?? CultureInfo.CurrentUICulture;
            }
        }

        /// <summary>
        /// Reads the version from assembly metadata: "2.0.0"
        /// </summary>
        public static string Version
        {
            get
            {
                if (_version == null)
                {
                    var ver = Assembly.GetExecutingAssembly().GetName().Version;
                    _version = ver != null ? $"{ver.Major}.{ver.Minor}.{ver.Build}" : "0.0.0";
                }
                return _version;
            }
        }

        /// <summary>Gets a localized string by resource key.</summary>
        public static string Get(string key)
        {
            // For zh-CN, try Chinese resource first, fall back to neutral (English)
            if (_culture.Name.StartsWith("zh"))
            {
                var zhValue = _resourcesZhCN.GetString(key, _culture);
                if (zhValue != null) return zhValue;
            }
            return _resources.GetString(key, _culture) ?? $"[[{key}]]";
        }

        /// <summary>Gets a localized format string and applies arguments.</summary>
        public static string Format(string key, params object[] args)
        {
            var format = Get(key);
            return args.Length > 0 ? string.Format(_culture, format, args) : format;
        }
    }
}
