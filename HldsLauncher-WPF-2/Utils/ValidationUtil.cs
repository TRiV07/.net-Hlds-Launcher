using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HldsLauncher.Utils
{
    public static class ValidationUtil
    {
        private static Regex _numberRegex = new Regex("[0-9]+");
        public static bool IsNumber(string text)
        {
            return _numberRegex.IsMatch(text);
        }
    }
}
