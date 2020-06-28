using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HldsLauncher.Enums
{
    public static class ConsoleTypes
    {
        private static IList<ConsoleTypeWrapper> _consoleTypesList = new List<ConsoleTypeWrapper>(new ConsoleTypeWrapper[]
        {
            new ConsoleTypeWrapper { Value = ConsoleType.Integrated, Name = Properties.Resources.aes_ConsoleTypeIntegrated },
            new ConsoleTypeWrapper { Value = ConsoleType.Native, Name = Properties.Resources.aes_ConsoleTypeNative },
        });
        public static IList<ConsoleTypeWrapper> ConsoleTypesList { get { return _consoleTypesList; } }
    }

    public class ConsoleTypeWrapper
    {
        public ConsoleType Value { get; set; }
        public string Name { get; set; }
    }
}
