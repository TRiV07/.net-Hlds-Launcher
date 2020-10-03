﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace HldsLauncher.Log
{
    public class NLogger : ILogger
    {
        private Logger _logger;
        public NLogger(string name)
        {
            _logger = NLog.LogManager.GetLogger(name);
        }

        public void Trace(string message)
        {
            _logger.Trace(message);
        }

        public void TraceException(string message, Exception exception)
        {
            _logger.TraceException(message, exception);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void DebugException(string message, Exception exception)
        {
            _logger.DebugException(message, exception);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void InfoException(string message, Exception exception)
        {
            _logger.InfoException(message, exception);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void WarnException(string message, Exception exception)
        {
            _logger.WarnException(message, exception);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void ErrorException(string message, Exception exception)
        {
            _logger.ErrorException(message, exception);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void FatalException(string message, Exception exception)
        {
            _logger.FatalException(message, exception);
        }
    }
}
