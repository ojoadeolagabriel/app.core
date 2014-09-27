using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using app.core.data.common.contract;
using app.core.util.reflection;

namespace app.core.data.common.builder
{
    /// <summary>
    /// DatasourceTypeManager Class.
    /// </summary>
    public static class DatasourceTypeManager
    {
        public const string HandlerLocation = "app.core.data.common.handler";

        /// <summary>
        /// Load Database Source Type Hander
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static IDatabaseSourceTypeHandler Load(XDocument doc)
        {
            try
            {
                var configElement = (XElement)doc.Elements().FirstOrDefault(c => c.Name == "configuration");
                if (configElement != null)
                {
                    var databaseSourceTypeHanderName = configElement.Descendants("data_handler").First().Attribute("type").Value;
                    var connectionString = configElement.Descendants("connection_string").First().Attribute("value").Value;
                    var databaseUnitName = configElement.Descendants("data_handler").First().Attribute("database_unit_name").Value;

                    var type = ReflectionHelper.GetTypeInNamespace(HandlerLocation, databaseSourceTypeHanderName);
                    var handler = (IDatabaseSourceTypeHandler)Activator.CreateInstance(type);
                    handler.ConnectionString = connectionString;
                    handler.DatabaseUnit = databaseUnitName;

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
