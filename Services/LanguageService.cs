using System.Text.Json;
using WebApplication1.Enums;

namespace WebApplication1.Services
{
    public static class LanguageService
    {
        private static readonly Dictionary<Languages, Dictionary<string, string>> _languagesData = new();
        private static bool _isInitialized = false;

        static LanguageService()
        {
            if (!_isInitialized)
            {
                LoadAllLanguages();
                _isInitialized = true;
            }
        }

        private static void LoadAllLanguages()
        {
            foreach (Languages language in Enum.GetValues(typeof(Languages)))
            {
                string languageFilePath = Path.Combine("Languages/", $"{language}.json");

                if (File.Exists(languageFilePath))
                {
                    var languageData = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(languageFilePath));
                    if (languageData != null)
                    {
                        _languagesData[language] = languageData;
                    }
                }
            }
        }

        public static string GetLocalizedText(this string key, Languages language = Languages.Azerbaijan)
        {
            if (_languagesData.ContainsKey(language) && _languagesData[language].ContainsKey(key))
            {
                return _languagesData[language][key];
            }
            return key;
        }
    }

}
