using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HldsLauncher.Enums
{
    public static class Languages
    {
        private static IList<LanguageWrapper> _languagesList = new List<LanguageWrapper>(new LanguageWrapper[]
        {
            new LanguageWrapper { Value = "en-US", Name = "English" },
            new LanguageWrapper { Value = "ru-RU", Name = "Русский" }
        });
        public static IList<LanguageWrapper> LanguagesList { get { return _languagesList; } }
    }

    public class LanguageWrapper
    {
        public string Value { get; set; }
        public string Name { get; set; }
    }
}
