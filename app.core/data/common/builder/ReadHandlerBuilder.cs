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
    /// ReadHandlerBuilder Class.
    /// </summary>
    public static class ReadHandlerBuilder
    {
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
                    var handlerElem = configElement.Element("handler");
                    if (handlerElem != null)
                    {
                        var databaseSourceTypeHanderName = handlerElem.Attribute("type").Value;
                        if (!string.IsNullOrEmpty(databaseSourceTypeHanderName))
                        {
                            var type = ReflectionHelper.GetTypeInNamespace("app.core.data.handler." + databaseSourceTypeHanderName);
                            var handler = (IDatabaseSourceTypeHandler)Activator.CreateInstance(type);
                            return handler;
                        }
                    }
                }
            }
            catch
            {
                //do nothing
            }

            var deftype = ReflectionHelper.GetTypeInNamespace("app.core.data.handler.MsSql");
            var defhandler = (IDatabaseSourceTypeHandler)Activator.CreateInstance(deftype);
            return defhandler;
        }
    }
}
