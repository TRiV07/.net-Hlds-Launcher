using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HldsLauncher.Enums
{
    public static class Priorities
    {
        private static IList<PriorityWrapper> _prioritiesList = new List<PriorityWrapper>(new PriorityWrapper[]
        {
            new PriorityWrapper { Value = ProcessPriorityClass.Idle, Name = Properties.Resources.aes_ServerPriorityIdle },
            new PriorityWrapper { Value = ProcessPriorityClass.BelowNormal, Name = Properties.Resources.aes_ServerPriorityBelowNormal },
            new PriorityWrapper { Value = ProcessPriorityClass.Normal, Name = Properties.Resources.aes_ServerPriorityNormal },
            new PriorityWrapper { Value = ProcessPriorityClass.AboveNormal, Name = Properties.Resources.aes_ServerPriorityAboveNormal },
            new PriorityWrapper { Value = ProcessPriorityClass.High, Name = Properties.Resources.aes_ServerPriorityHigh },
            new PriorityWrapper { Value = ProcessPriorityClass.RealTime, Name = Properties.Resources.aes_ServerPriorityRealTime },
        });
        public static IList<PriorityWrapper> PrioritiesList { get { return _prioritiesList; } }
    }

    public class PriorityWrapper
    {
        public ProcessPriorityClass Value { get; set; }
        public string Name { get; set; }
    }
}
