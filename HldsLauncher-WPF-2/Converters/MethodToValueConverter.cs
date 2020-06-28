﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace HldsLauncher.Converters
{
    public sealed class MethodToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var methodName = parameter as string;
            string[] tokens;
            while ((tokens = methodName.Split('.')).Length > 1)
            {
                var propertyName = tokens[0];
                methodName = tokens[1];
                value = value.GetType().GetProperty(propertyName).GetValue(value, null);
            }
            if (value == null || methodName == null)
                return value;
            var methodInfo = value.GetType().GetMethod(methodName, new Type[0]);
            if (methodInfo == null)
                return value;
            return methodInfo.Invoke(value, new object[0]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MethodToValueConverter can only be used for one way conversion.");
        }
    }
}
