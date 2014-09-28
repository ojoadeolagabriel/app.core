using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
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
            entity.SetId(id);

            //get primary key
            var primaryKey = entity.EntityInfo.PrimaryKeyInfo.columnDescription;

            //select query
            var selectQuery = SpBuilder.BuildRetrieveByIdSp(entity.TableName, _handler.IgnoreTablePrefixes);

            //build params
            var param = new List<SqlParameter> { new SqlParameter("@" + primaryKey, entity.Id) };

            //exec unique
            ExecuteUniqueQuery(entity, selectQuery, param);
            return entity;
        }

        public List<TEntity> RetreiveAll()
        {
            return null;
        }


        /// <summary>
        /// Execute Unique Query
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="selectQuery"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        private TEntity ExecuteUniqueQuery(TEntity entity, string selectQuery, List<SqlParameter> sqlParameters)
        {
            //exec
            entity = (TEntity)_handler.ExecuteUniqueSp(entity, sqlParameters, selectQuery);
            return entity;
        }

        private T ExecuteNonQuery<T>(TEntity entity)
        {
            return default(T);
        }

        public TEntity Persist(TEntity entity)
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
