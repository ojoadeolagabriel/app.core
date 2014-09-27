using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using app.core.data.common.builder.contract;
using app.core.data.common.contract;
using app.core.util.reflection;

namespace app.core.data.common.builder
{
    /// <summary>
    /// DataSourceTypeManager Class.
    /// </summary>
    public static class DataSourceTypeManager
    {
        public const string HandlerLocation = "app.core.data.common.handler";

        /// <summary>
        /// ReadXmlConfiguration Database Source Type Hander
        /// </summary>
        /// <param name="databaseUnit"></param>
        /// <returns></returns>
        public static IDatabaseSourceTypeHandler ReadXmlConfiguration(string databaseUnit = null)
        {
            try
            {
                var xmlConfig = @"db_config.xml";
                xmlConfig = Path.GetFullPath(xmlConfig);
                var doc = XDocument.Load(xmlConfig);

                var configElement = (XElement)doc.Elements().FirstOrDefault(c => c.Name == "configuration");
                if (configElement != null)
                {
                    //fetch configs
                    var databaseSourceTypeHanderName = configElement.Descendants("data_handler").First().Attribute("type").Value;
                    var connectionString = configElement.Descendants("connection_string").First().Attribute("value").Value;
                    var databaseUnitName = databaseUnit ?? configElement.Descendants("data_handler").First().Attribute("database_unit_name").Value;
                    var ignoreTablePrefixes = configElement.Descendants("ignore_table_prefixes").First().Attribute("value").Value;

                    var type = ReflectionHelper.GetTypeInNamespace(HandlerLocation, databaseSourceTypeHanderName);
                    var handler = (IDatabaseSourceTypeHandler)Activator.CreateInstance(type);

                    //set params
                    handler.ConnectionString = connectionString;
                    handler.DatabaseUnit = databaseUnitName;
                    handler.IgnoreTablePrefixes = ignoreTablePrefixes.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                    return handler;
                }
            }
            catch (Exception exc)
            {
                var errMsg = exc.Message;
            }

            var deftype = ReflectionHelper.GetTypeInNamespace(HandlerLocation, "MsSql");
            var defhandler = (IDatabaseSourceTypeHandler)Activator.CreateInstance(deftype);
            return defhandler;
        }
    }
}
