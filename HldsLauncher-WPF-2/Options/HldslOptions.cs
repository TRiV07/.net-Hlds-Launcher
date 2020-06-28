using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Reflection;
using System.Globalization;
using HldsLauncher.Utils;
using System.Xml;

namespace HldsLauncher.Options
{
    public class HldslOptions
    {
        public int StartInterval { get; set; }
        public int NtTimerResolution { get; set; }
        public bool StartServersAfterProgramStart { get; set; }
        public int WaitBeforeServersStart { get; set; }
        public string Language { get; set; }
        public bool ShowServerInfoInTabs { get; set; }
        public bool ShowTabsOnlyIfServerStarted { get; set; }
        public bool MMTimerStartUpSet { get; set; }
        public XElement Servers { get; set; }
        public XElement Commands { get; set; }

        public HldslOptions()
        {
            StartServersAfterProgramStart = false;
            ShowServerInfoInTabs = false;
            WaitBeforeServersStart = 30;
            Language = "en-US";
            StartInterval = 3;
            NtTimerResolution = NtTimerResolutionUtil.Actual;
            Servers = new XElement("Servers");
            Commands = new XElement("Commands");
        }

        public void SaveToFile(string fileName)
        {
            XElement xml = (XElement)GetXml();
            xml.Save(fileName);
        }

        public static HldslOptions LoadFromFile(string fileName)
        {
            XElement xOptions = XElement.Load(fileName);
            return CreateFromXml(xOptions);
        }

        public XNode GetXml()
        {
            XElement xOptions = new XElement("Options");
            Type optionsType = this.GetType();
            foreach (PropertyInfo propertyInfo in optionsType.GetProperties())
            {
                XElement xProp;
                if (propertyInfo.PropertyType != typeof(XElement))
                {
                    xProp = new XElement(propertyInfo.Name);
                    xProp.Value = propertyInfo.GetValue(this, null).ToString();
                }
                else
                {
                    xProp = ((XElement)propertyInfo.GetValue(this, null));
                }
                xOptions.Add(xProp);
                //prop.SetValue(serverOptions, xServer.XPathSelectElement(prop.Name), null);
            }
            return xOptions;
        }

        public static HldslOptions CreateFromXml(XNode xOptions)
        {
            //GoldSourceServerOptions serverOptions = new GoldSourceServerOptions();
            HldslOptions serverOptions = new HldslOptions();
            //T serverOptions = new T();
            //Type goldSourceServerOptionsType = serverOptions.GetType();
            Type optionsType = typeof(HldslOptions);
            foreach (PropertyInfo propertyInfo in optionsType.GetProperties())
            {
                string strValue = "";
                try
                {
                    if (xOptions.XPathSelectElement(propertyInfo.Name).HasElements)
                    {
                        var value = Convert.ChangeType(xOptions.XPathSelectElement(propertyInfo.Name), propertyInfo.PropertyType, CultureInfo.CurrentCulture);
                        propertyInfo.SetValue(serverOptions, value, null);
                    }
                    else
                    {
                        strValue = xOptions.XPathSelectElement(propertyInfo.Name).Value;
                    }
                }
                catch (NullReferenceException)
                {
                    continue;
                }

                if (strValue == "")
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
    }
}
