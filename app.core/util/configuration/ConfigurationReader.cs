using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;

namespace app.core.util.configuration
{
    /// <summary>
    /// ConfigurationReader Class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConfigurationReader<T>
    {
        /// <summary>
        /// Read data
        /// </summary>
        /// <param name="key"></param>
        /// <param name="group"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Read(string key,string group = null,T defaultValue = default (T))
        {
            try
            {
                if (string.IsNullOrEmpty(group))
                {
                    return (T)Convert.ChangeType(ConfigurationManager.AppSettings[key], typeof(T));
                }

                var data = ((NameValueCollection)ConfigurationManager.GetSection(group))[key];
                return (T)Convert.ChangeType(data, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
