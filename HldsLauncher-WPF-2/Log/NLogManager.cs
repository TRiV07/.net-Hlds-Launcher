using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NLog.Config;
using NLog.Targets;
using NLog.Layouts;
using NLog;
using System.Data.Common;
using NLog.Targets.Wrappers;
using HldsLauncher.Utils;

namespace HldsLauncher.Log
{
    public class NLogManager : ILogManager
    {
        public NLogManager()
        {
            LoggingConfiguration config = new LoggingConfiguration();
            
            FileTarget fileTargetFullLog = new FileTarget();
            fileTargetFullLog.FileName = CommonUtils.GetProgramDirectory() + "/Logs/${date:format=yyyy-MM-dd}/Full.log";
            fileTargetFullLog.Layout = "${date:format=dd.MM.yyyy HH\\:mm\\:ss} (${level:uppercase=true}) (${logger}): ${message} ${exception:format=ToString}";
            fileTargetFullLog.AutoFlush = true;
            config.AddTarget("fullLog", fileTargetFullLog);
            LoggingRule fullLogRule = new LoggingRule("*", LogLevel.Trace, fileTargetFullLog);
            config.LoggingRules.Add(fullLogRule);

            FileTarget fileTargetShortLog = new FileTarget();
            fileTargetShortLog.FileName = CommonUtils.GetProgramDirectory() + "/Logs/${date:format=yyyy-MM-dd}/Short.log";
            fileTargetShortLog.Layout = "${date:format=dd.MM.yyyy HH\\:mm\\:ss} (${level:uppercase=true}): ${message}";
            fileTargetShortLog.AutoFlush = true;
            config.AddTarget("shortLog", fileTargetShortLog);
            LoggingRule shortLogRule = new LoggingRule("*", LogLevel.Info, fileTargetShortLog);
            config.LoggingRules.Add(shortLogRule);

            WpfRichTextBoxTarget richTextBoxTarget = new WpfRichTextBoxTarget();
            richTextBoxTarget.AutoScroll = true;
            richTextBoxTarget.ControlName = "richTextBoxLog";
            richTextBoxTarget.FormName = "Form1";
            richTextBoxTarget.UseDefaultRowColoringRules = true;
            richTextBoxTarget.Layout = "${date:format=dd.MM.yyyy HH\\:mm\\:ss} (${level:uppercase=true}): ${message}";
            config.AddTarget("richTextBoxLog", richTextBoxTarget);
            LoggingRule richTextBoxLogRule = new LoggingRule("*", LogLevel.Trace, richTextBoxTarget);
            config.LoggingRules.Add(richTextBoxLogRule);

            //AsyncTargetWrapper asyncWrapper = new AsyncTargetWrapper();
            //asyncWrapper.Name = "AsyncRichTextBox";
            //asyncWrapper.WrappedTarget = richTextBoxTarget;
            //SimpleConfigurator.ConfigureForTargetLogging(asyncWrapper, LogLevel.Info);
            

            

            NLog.LogManager.Configuration = config;
        }

        public ILogger GetCurrentClassLogger()
        {
            StackFrame frame = new StackFrame(1, false);
            return new NLogger(frame.GetMethod().DeclaringType.FullName);
        }

        public ILogger GetLogger(string name)
        {
            return new NLogger(name);
        }
    }
}
