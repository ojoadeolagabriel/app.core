using System;
using System.IO;
using System.Xml.Linq;
using app.core.data.common.builder;
using app.core.data.common.builder.contract;
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
            _handler = DataSourceTypeManager.ReadXmlConfiguration(DatabaseUnit);
        }

        /// <summary>
        /// Retreive By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity RetreiveById(TId id)
        {
            //get entity
            var entity = Activator.CreateInstance<TEntity>();
            //get primary key
            var primaryKey = entity.EntityInfo.PrimaryKeyInfo.columnDescription;
            //get columns
            var otherColumns = entity.EntityInfo.MapColumns;
            //select query
            var selectQuery = SpBuilder.RetreiveUniqueSp(entity.SchemaName, _handler.IgnoreTablePrefixes);


            return entity;
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
