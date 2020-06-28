using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using HldsLauncher.Enums;

namespace HldsLauncher.Options
{
    public abstract class BaseServerOptions : IServerOptions, INotifyPropertyChanged
    {
        private static List<string> _ignoredProps = new List<string> { "CommandLineProp" };

        private string _executablePath;
        public string ExecutablePath { get { return _executablePath; } set { _executablePath = value; OnPropertyChanged("ExecutablePath"); } }

        private ProcessPriorityClass _priority;
        public ProcessPriorityClass Priority { get { return _priority; } set { _priority = value; OnPropertyChanged("Priority"); } }

        private IntPtr _processorAffinity;
        public IntPtr ProcessorAffinity { get { return _processorAffinity; } set { _processorAffinity = value; OnPropertyChanged("ProcessorAffinity"); } }

        private bool _autoRestart;
        public bool AutoRestart { get { return _autoRestart; } set { _autoRestart = value; OnPropertyChanged("AutoRestart"); } }

        private bool _crashCountLimit;
        public bool CrashCountLimit { get { return _crashCountLimit; } set { _crashCountLimit = value; OnPropertyChanged("CrashCountLimit"); } }

        private int _maxCrashCount;
        public int MaxCrashCount { get { return _maxCrashCount; } set { _maxCrashCount = value; OnPropertyChanged("MaxCrashCount"); } }

        private int _crashCountTime;
        public int CrashCountTime { get { return _crashCountTime; } set { _crashCountTime = value; OnPropertyChanged("CrashCountTime"); } }

        private bool _activeServer;
        public bool ActiveServer { get { return _activeServer; } set { _activeServer = value; OnPropertyChanged("ActiveServer"); } }

        private ServerType _type;
        public ServerType Type { get { return _type; } set { _type = value; OnPropertyChanged("Type"); } }

        private string _additionalCommandLineArgs;
        public string AdditionalCommandLineArgs { get { return _additionalCommandLineArgs; } set { _additionalCommandLineArgs = value; OnPropertyChanged("AdditionalCommandLineArgs"); } }

        private string _hostName;
        public string HostName { get { return _hostName; } set { _hostName = value; OnPropertyChanged("HostName"); } }

        protected BaseServerOptions()
        {
            ExecutablePath = "";
            Priority = ProcessPriorityClass.AboveNormal;
            ProcessorAffinity = (IntPtr)0;
            AutoRestart = true;
            CrashCountLimit = true;
            MaxCrashCount = 5;
            CrashCountTime = 60;
            ActiveServer = true;
            AdditionalCommandLineArgs = "";
            HostName = "and1gaming.org.ua";
        }

        public virtual string CommandLine()
        {
            return ((!string.IsNullOrWhiteSpace(AdditionalCommandLineArgs)) ? " " + AdditionalCommandLineArgs : "");
        }

        public string CommandLineProp { get { return CommandLine(); } }

        public static T CreateFromXml<T>(XNode xServer)
            where T : IServerOptions//, new()
        {
            //GoldSourceServerOptions serverOptions = new GoldSourceServerOptions();
            T serverOptions = Activator.CreateInstance<T>();
            //T serverOptions = new T();
            //Type goldSourceServerOptionsType = serverOptions.GetType();
            Type optionsType = typeof(T);
            foreach (PropertyInfo propertyInfo in optionsType.GetProperties())
            {
                if (_ignoredProps.Contains(propertyInfo.Name)) continue;
                string strValue = "";
                try
                {
                    strValue = xServer.XPathSelectElement(propertyInfo.Name).Value;
                }
                catch (NullReferenceException)
                {
                    continue;
                }

                if (strValue == "" && propertyInfo.PropertyType != typeof(string))
                {
                    continue;
                }
                if (propertyInfo.PropertyType.IsEnum) //enum
                {
                    var value = Enum.Parse(propertyInfo.PropertyType, strValue);
                    propertyInfo.SetValue(serverOptions, value, null);
                }
                else if (propertyInfo.PropertyType == typeof(string)) //string
                {
                    propertyInfo.SetValue(serverOptions, strValue, null);
                }
                else if (propertyInfo.PropertyType == typeof(IntPtr))
                {
                    var value = (IntPtr)int.Parse(strValue, CultureInfo.CurrentCulture);
                    propertyInfo.SetValue(serverOptions, value, null);
                }
                else
                {
                    var value = Convert.ChangeType(strValue, propertyInfo.PropertyType, CultureInfo.CurrentCulture);
                    propertyInfo.SetValue(serverOptions, value, null);
                }
            }
            return serverOptions;
        }

        public XNode GetXml()
        {
            XElement xServer = new XElement("Server");
            Type optionsType = this.GetType();
            foreach (PropertyInfo propertyInfo in optionsType.GetProperties())
            {
                if (_ignoredProps.Contains(propertyInfo.Name)) continue;
                XElement xProp = new XElement(propertyInfo.Name);
                var propertyValue = propertyInfo.GetValue(this, null);
                if (propertyValue != null)
                {
                    xProp.Value = propertyValue.ToString();
                }
                xServer.Add(xProp);
            }
            return xServer;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                PropertyChanged(this, new PropertyChangedEventArgs("CommandLineProp"));
            }
        }
    }
}