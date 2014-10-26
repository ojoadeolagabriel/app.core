using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
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
        private static IDatabaseSourceTypeHandler _handler = null;

        public const string HandlerLocation = "app.core.data.common.handler";

        /// <summary>
        /// ReadXmlConfiguration Database Source Type Hander
        /// </summary>
        /// <param name="s"></param>
        /// <param name="databaseUnit"></param>
        /// <param name="rigKey"></param>
        /// <returns></returns>
        public static IDatabaseSourceTypeHandler ReadXmlConfiguration(string databaseUnit, string rigKey)
        {
            try
            {
                if (_handler != null)
                    return _handler;

                var xmlConfigurationPath = ConfigurationManager.AppSettings["rig.db.configuration.path"] ?? "db_config.xml";
                xmlConfigurationPath = HttpContext.Current != null ? HttpContext.Current.Server.MapPath(xmlConfigurationPath) : Path.GetFullPath(xmlConfigurationPath);

                var doc = XDocument.Load(xmlConfigurationPath);

                var configElement = doc.Elements().FirstOrDefault(c => c.Name == "configuration");
                if (configElement != null)
                {
                    var configRigElement = configElement.Elements("rig").FirstOrDefault(c => c.Attribute("value").Value == rigKey);
                    if (configRigElement != null)
                    {
                        //fetch configs
                        var dialect = XmlHelper<string>.ReadField(configElement, "connection.dialect", "value");
                        var connectionString = XmlHelper<string>.ReadField(configElement, "connection.string", "value");
                        var databaseUnitName = XmlHelper<string>.ReadField(configElement, "connection.databaseunit", "value", databaseUnit, true);
                        var ignoreTablePrefixes = XmlHelper<string>.ReadField(configElement, "ignore_table_prefixes", "value");
                        var autoGenerateCrudSql = XmlHelper<bool>.ReadField(configElement, "auto_generate_crud_sql", "value");
                        var forceUseOfExistingSp = XmlHelper<bool>.ReadField(configElement, "force_use_of_existing_sp", "value");
                        var ignoreForeignRelationship = XmlHelper<bool>.ReadField(configElement, "ignore_foreign_relationship", "value");
                        var auditCrudEnabled = XmlHelper<bool>.ReadField(configElement, "audit_crud_enabled", "value");

                        var type = ReflectionHelper.GetTypeInNamespace(HandlerLocation, dialect);
                        var handler = (IDatabaseSourceTypeHandler)Activator.CreateInstance(type);

                        //set params
                        handler.ConnectionString = connectionString;
                        handler.AuditCrudEnabled = auditCrudEnabled;
                        handler.IgnoreForeignRelationship = ignoreForeignRelationship;
                        handler.ForceUseOfExistingSp = forceUseOfExistingSp;
                        handler.DatabaseUnit = databaseUnitName;
                        handler.AutoGenerateCrudSql = autoGenerateCrudSql;
                        handler.IgnoreTablePrefixes = ignoreTablePrefixes.Split(new[] { ";", "|", "~" }, StringSplitOptions.RemoveEmptyEntries);

                        return handler;
                    }
                }
            }
            catch (Exception exc)
            {
                var errMsg = exc.Message;
                _handler = null;
            }

            var deftype = ReflectionHelper.GetTypeInNamespace(HandlerLocation, "MsSql");
            var defhandler = (IDatabaseSourceTypeHandler)Activator.CreateInstance(deftype);
            return defhandler;
        }
    }
}
