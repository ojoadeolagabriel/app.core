using System.IO;
using System.Xml.Linq;
using app.core.data.common.builder;
using app.core.data.common.contract;

namespace app.core.data.common.core
{
    /// <summary>
    /// CoreDao Class
    /// </summary>
    public class CoreDao<TId, TEntity> : ICoreDao<TId, TEntity>
        where TEntity : Entity<TId, TEntity>
    {
        ///
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

            var xmlConfig = @"db_config.xml";
            xmlConfig = Path.GetFullPath(xmlConfig);

            var xmlData = XDocument.Load(xmlConfig);
            _handler = ReadHandlerBuilder.Load(xmlData);
        }

        public TEntity RetreiveById(TId id)
        {
            //build sp
            return null;
        }

        public TEntity RetreiveAll()
        {
            return null;
        }

        public TEntity Save(TEntity entity)
        {
            return null;
        }

        public void Update(TEntity entity)
        {

        }

        public void Delete(TEntity entity)
        {

        }
    }
}
