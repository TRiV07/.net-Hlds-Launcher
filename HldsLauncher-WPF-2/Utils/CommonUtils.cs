using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Reflection;

namespace HldsLauncher.Utils
{
    public static class CommonUtils
    {
        public static string GetProgramDirectory()
        {
            return Path.GetDirectoryName(App.ResourceAssembly.Location);
        }

        public static T CloneObjectProps<T>(this object source, T destination)
        {
            foreach (var pSource in source.GetType().GetProperties())
            {
                var pDestination = destination.GetType().GetProperties().FirstOrDefault(x => x.Name == pSource.Name);
                if (pDestination != null)
                {
                    MethodInfo setMethod = pDestination.GetSetMethod();
                    MethodInfo getMethod = pSource.GetGetMethod();
                    if (getMethod != null && setMethod != null)
                    {
                        setMethod.Invoke(destination, new object[] { getMethod.Invoke(source, null) });
                    }
                }
            };
            return destination;
        }
    }
}
