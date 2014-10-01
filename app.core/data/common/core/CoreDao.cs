using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using app.core.data.common.builder;
using app.core.data.common.builder.contract;
using app.core.data.common.contract;
using app.core.exception.management;
using app.core.exception.management.code;
using app.core.util.reflection;

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
            var result = ExecuteUniqueQuery<TEntity>(selectQuery, param);
            return (TEntity)result;
        }

        public List<IEntity> RetreiveAll()
        {
            //get entity
            var entity = Activator.CreateInstance<TEntity>();

            //select query
            var selectQuery = SpBuilder.BuildRetrieveAllSp(entity.TableName, _handler.IgnoreTablePrefixes);

            //exec unique
            var data = ExecuteQuery<TEntity>(selectQuery, null);
            return data;
        }


        /// <summary>
        /// Execute Unique Query
        /// </summary>
        /// <param name="selectQuery"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public IEntity ExecuteUniqueQuery<TDto>(string selectQuery, List<SqlParameter> sqlParameters)
        {
            //exec
            var entity = _handler.ExecuteUniqueSp<TDto>(sqlParameters, selectQuery);
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectQuery"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public List<IEntity> ExecuteQuery<TDto>(string selectQuery, List<SqlParameter> sqlParameters)
        {
            //exec
            var data = _handler.ExecuteSp<TDto>(sqlParameters, selectQuery);
            return data;
        }

        /// <summary>
        /// Execute Non Query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public T ExecuteNonQuery<T>(string query, List<SqlParameter> sqlParameters)
        {
            var result = _handler.ExecuteNonQuery<T>(query, sqlParameters);
            return result;
        }

        public TEntity Persist(TEntity entity)
        {
            if (!entity.IsNew)
                throw new Exception("[ Data Is Not New ] - Cannot persist data");

            //get primary key
            var nonForeignColumns = entity.EntityInfo.MapColumns.Where(c => !c.Value.IsForeign);
            var foreignColumns = entity.EntityInfo.MapColumns.Where(c => c.Value != null && c.Value.IsForeign);

            //build persist query
            var parameters = new List<SqlParameter>();
            foreach (var column in nonForeignColumns)
                parameters.Add(new SqlParameter(string.Format("@{0}", column.Value._columnDescription), entity.GetType().GetProperty(column.Key).GetValue(entity, null)));

            foreach (var column in foreignColumns)
            {
                var childData = (IEntity)entity.GetType().GetProperty(column.Key).GetValue(entity, null);

                if (childData.IsNew)
                    ExceptionHandler.Handle(string.Format("{0} is orphaned, canceling save request", childData.TableName), ExceptionCode.DataPersistenceIssue);

                parameters.Add(new SqlParameter(string.Format("@{0}", column.Value._columnDescription),
                    column.Value._type.GetProperty("Id").GetValue(childData, null)));
            }

            //select query
            var persistQuery = SpBuilder.BuildPersistSp(entity.TableName, _handler.IgnoreTablePrefixes);

            var primaryKeyIndex = GetType()
                        .GetMethod("ExecuteNonQuery")
                        .MakeGenericMethod(entity.PrimaryKeyInfo.ColumnType)
                        .Invoke(this, new object[] { persistQuery, parameters });

            entity.SetId((TId)Convert.ChangeType(primaryKeyIndex, typeof(TId)));
            return null;
        }

        public void Update(TEntity entity)
        {
            if (entity.IsNew)
                throw new Exception("[ Data Is New ] - Cannot persist data");

            //get primary key
            var nonForeignColumns = entity.EntityInfo.MapColumns.Where(c => !c.Value.IsForeign);
            var foreignColumns = entity.EntityInfo.MapColumns.Where(c => c.Value != null && c.Value.IsForeign);

            //build persist query
            var parameters = new List<SqlParameter>();

            //add primary key
            parameters.Add(new SqlParameter("@Id", entity.Id));

            foreach (var column in nonForeignColumns)
            {
                parameters.Add(new SqlParameter(string.Format("@{0}", column.Value._columnDescription),
                    entity.GetType().GetProperty(column.Key).GetValue(entity, null)));
            }

            foreach (var column in foreignColumns)
            {
                var childData = (IEntity)entity.GetType().GetProperty(column.Key).GetValue(entity, null);

                if (childData.IsNew)
                    ExceptionHandler.Handle(string.Format("{0} is orphaned, canceling update request", childData.TableName), ExceptionCode.DataUpdateIssue);

                parameters.Add(new SqlParameter(string.Format("@{0}", column.Value._columnDescription),
                    column.Value._type.GetProperty("Id").GetValue(childData, null)));
            }

            //select query
            var persistQuery = SpBuilder.BuildUpdateSp(entity.TableName, _handler.IgnoreTablePrefixes);

            var result = GetType()
                        .GetMethod("ExecuteNonQuery")
                        .MakeGenericMethod(typeof(IEntity))
                        .Invoke(this, new object[] { persistQuery, parameters });
        }

        public void Delete(TEntity entity)
        {

        }
    }
}
