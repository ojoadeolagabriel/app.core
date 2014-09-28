using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using app.core.data.common.builder.contract;
using app.core.data.common.contract;
using app.core.util.reflection;
using app.core.util.xml;

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
                var xmlConfigurationPath = @"db_config.xml";
                xmlConfigurationPath = Path.GetFullPath(xmlConfigurationPath);
                var doc = XDocument.Load(xmlConfigurationPath);

                var configElement = (XElement)doc.Elements().FirstOrDefault(c => c.Name == "configuration");
                if (configElement != null)
                {
                    //fetch configs
                    var dialect = XmlHelper<string>.ReadField(configElement, "connection.dialect", "value");
                    var connectionString = XmlHelper<string>.ReadField(configElement, "connection.string", "value");
                    var databaseUnitName = XmlHelper<string>.ReadField(configElement, "connection.databaseunit", "value", databaseUnit, true);
                    var ignoreTablePrefixes = XmlHelper<string>.ReadField(configElement, "ignore_table_prefixes", "value");

                    var type = ReflectionHelper.GetTypeInNamespace(HandlerLocation, dialect);
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
