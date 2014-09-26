using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Xml.Linq;
using app.core.data.common.builder;
using app.core.data.common.contract;

namespace app.core.data.common
{
    /// <summary>
    /// CoreDao Class
    /// </summary>
    public class CoreDao<TId, TEntity> : ICoreDao<TId, TEntity> 
        where TEntity : Entity<TId>
    {
        private IDatabaseSourceTypeHandler _handler;

        /// <summary>
        /// DatabaseUnit
        /// </summary>
        public string DatabaseUnit { get; set; }

        /// <summary>
        /// Class consrtructor.
        /// </summary>
        /// <param name="databaseUnit"></param>
        public CoreDao(string databaseUnit)
        {
            DatabaseUnit = databaseUnit;
            ReadHandlerConfiguration();
        }

        /// <summary>
        /// Read Handler Configuration
        /// </summary>
        private void ReadHandlerConfiguration()
        {
            if (_handler != null) return;

            const string xmlConfig = "db_config.xml";
            if (File.Exists(xmlConfig))
            {
                var xmlData = XDocument.Load(xmlConfig);
                _handler = ReadHandlerBuilder.Load(xmlData);
            }
        }

        
    }
}
