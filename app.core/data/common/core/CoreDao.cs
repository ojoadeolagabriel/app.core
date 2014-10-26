using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using app.core.data.common.builder;
using app.core.data.common.builder.contract;
using app.core.data.common.contract;
using app.core.data.common.core.relation;
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
        #region Support Members
        private static IDatabaseSourceTypeHandler _handler;

        /// <summary>
        /// DatabaseUnit
        /// </summary>
        public static string DatabaseUnit { get; set; }

        /// <summary>
        /// Class consrtructor.
        /// </summary>
        /// <param name="databaseUnit"></param>
        /// <param name="rigKey"></param>
        public CoreDao(string databaseUnit, string rigKey = "core.db")
        {
            DatabaseUnit = databaseUnit;
            ReadHandlerConfiguration(rigKey);
        }

        /// <summary>
        /// Read Handler Configuration
        /// </summary>
        /// <param name="rigKey"></param>
        private void ReadHandlerConfiguration(string rigKey)
        {
            if (_handler != null) return;
            _handler = DataSourceTypeManager.ReadXmlConfiguration(DatabaseUnit, rigKey);
        }

        /// <summary>
        /// Execute Unique Query
        /// </summary>
        /// <param name="selectQuery"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public TDto ExecuteUniqueQuery<TDto>(string selectQuery, List<SqlParameter> sqlParameters)
        {
            var entity = _handler.ExecuteUniqueSp<TDto>(sqlParameters, selectQuery);
            return (TDto)entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectQuery"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public List<TDto> ExecuteQuery<TDto>(string selectQuery, List<SqlParameter> sqlParameters)
        {
            //exec
            var data = _handler.ExecuteSp<TDto>(sqlParameters, selectQuery);
            return data.OfType<TDto>().ToList();
        }

        /// <summary>
        /// Execute Non Query
        /// </summary>am>
        /// <param name="query"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        /// <typeparam name="T"></typeparam>
        public T ExecuteNonQuery<T>(string query, List<SqlParameter> sqlParameters)
        {
            var result = _handler.ExecuteNonQuery<T>(query, sqlParameters);
            return result;
        }
        #endregion

        private Dictionary<string, object> BuildListOfMappedColumnsCreate(TEntity entity, bool addBrackets = false, bool addPrimaryKeyInfo = false)
        {
            try
            {
                //get primary key
                var nonForeignColumns = entity.EntityInfo.MapColumns.Where(c => !c.Value.IsForeign);
                var foreignColumns = entity.EntityInfo.MapColumns.Where(c => c.Value != null && c.Value.IsForeign);

                var cols = new Dictionary<string, object>();

                if (addPrimaryKeyInfo)
                    cols.Add(!addBrackets ? entity.PrimaryKeyInfo.columnDescription : string.Format("[{0}]", entity.PrimaryKeyInfo.columnDescription), "");

                nonForeignColumns.ToList().ForEach(c => cols.Add(c.Value._columnDescription, SetColumnValue(c, entity)));
                foreignColumns.ToList().ForEach(c => cols.Add(!addBrackets ? c.Value._columnDescription : string.Format("[{0}]", c.Value._columnDescription), SetColumnValue(c, entity)));

                return cols;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex.Message, ExceptionCode.UnKnownError, ex);
            }
            return null;
        }

        private Dictionary<string, object> BuildListOfMappedColumnsUpdate(TEntity entity, bool addBrackets = false, bool addPrimaryKeyInfo = false)
        {
            try
            {
                //get primary key
                var nonForeignColumns = entity.EntityInfo.MapColumns.Where(c => !c.Value.IsForeign);
                var foreignColumns = entity.EntityInfo.MapColumns.Where(c => c.Value != null && c.Value.IsForeign);

                var cols = new Dictionary<string, object>();

                nonForeignColumns.ToList().ForEach(c => cols.Add(c.Value._columnDescription, SetColumnValue(c, entity)));
                foreignColumns.ToList().ForEach(c => cols.Add(!addBrackets ? c.Value._columnDescription : string.Format("[{0}]", c.Value._columnDescription), SetColumnValue(c, entity)));

                return cols;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex.Message, ExceptionCode.UnKnownError, ex);
            }
            return null;
        }

        private object SetColumnValue(KeyValuePair<string, ColumnInfo> c, TEntity entity)
        {
            try
            {
                var castEntity = (IEntity)entity;
                var castEntityType = castEntity.GetType();
                var prop = castEntityType.GetProperty(c.Key);
                var childData = prop.GetValue(entity, null);

                return childData;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #region CRUD Operations
        public TEntity Persist(TEntity entity)
        {
            if (!entity.IsNew)
                throw new Exception("[ Data Is Not New ] - Cannot persist data");

            //get primary key
            var nonForeignColumns = entity.EntityInfo.MapColumns.Where(c => !c.Value.IsForeign);
            var foreignColumns = entity.EntityInfo.MapColumns.Where(c => c.Value != null && c.Value.IsForeign);

            //if auto gen
            if (_handler.AutoGenerateCrudSql)
            {
                var props = BuildListOfMappedColumnsCreate(entity);
                var query = SpBuilder.BuildPersistSp(entity.SchemaName, _handler.IgnoreTablePrefixes, _handler, props);

                var data = GetType()
                        .GetMethod("ExecuteNonQuery")
                        .MakeGenericMethod(entity.PrimaryKeyInfo.ColumnType)
                        .Invoke(this, new object[] { query, null });

                entity.SetId(data);
                return entity;
            }

            //build persist query
            var parameters = new List<SqlParameter>();
            foreach (var column in nonForeignColumns)
                parameters.Add(new SqlParameter(string.Format("@{0}", column.Value._columnDescription), entity.GetType().GetProperty(column.Key).GetValue(entity, null)));

            foreach (var column in foreignColumns)
            {
                var childData = (IEntity)entity.GetType().GetProperty(column.Key).GetValue(entity, null);

                if (childData == null || childData.IsNew)
                    ExceptionHandler.Handle(string.Format("{0} is orphaned, cancelling save request", column.Key), ExceptionCode.DataPersistenceIssue);

                parameters.Add(new SqlParameter(string.Format("@{0}", column.Value._columnDescription),
                    column.Value._type.GetProperty("Id").GetValue(childData, null)));
            }

            //select query
            var persistQuery = SpBuilder.BuildPersistSp(entity.SchemaName, _handler.IgnoreTablePrefixes);

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
                ExceptionHandler.Handle("Dto is new, cannot call update of orphaned object", ExceptionCode.DataPersistenceIssue);

            //get primary key
            var nonForeignColumns = entity.EntityInfo.MapColumns.Where(c => !c.Value.IsForeign);
            var foreignColumns = entity.EntityInfo.MapColumns.Where(c => c.Value != null && c.Value.IsForeign);

            //build persist query
            var parameters = new List<SqlParameter> { new SqlParameter("@Id", entity.Id) };

            //add primary key

            if (_handler.AutoGenerateCrudSql)
            {
                var props = BuildListOfMappedColumnsUpdate(entity, false, true);
                var query = SpBuilder.BuildUpdateSp(entity.SchemaName, _handler.IgnoreTablePrefixes, _handler, props, entity.PrimaryKeyInfo.columnDescription, entity.Id.ToString());

                GetType()
                       .GetMethod("ExecuteNonQuery")
                       .MakeGenericMethod(typeof(IEntity))
                       .Invoke(this, new object[] { query, null });

                return;
            }

            foreach (var column in nonForeignColumns)
            {
                parameters.Add(new SqlParameter(string.Format("@{0}", column.Value._columnDescription),
                    entity.GetType().GetProperty(column.Key).GetValue(entity, null)));
            }

            foreach (var column in foreignColumns)
            {
                var childData = (IEntity)entity.GetType().GetProperty(column.Key).GetValue(entity, null);

                if (childData.IsNew)
                    ExceptionHandler.Handle(string.Format("{0} is orphaned, canceling update request", childData.SchemaName), ExceptionCode.DataUpdateIssue);

                parameters.Add(new SqlParameter(string.Format("@{0}", column.Value._columnDescription),
                    column.Value._type.GetProperty("Id").GetValue(childData, null)));
            }

            //select query
            var persistQuery = SpBuilder.BuildUpdateSp(entity.SchemaName, _handler.IgnoreTablePrefixes);

            GetType()
                        .GetMethod("ExecuteNonQuery")
                        .MakeGenericMethod(typeof(IEntity))
                        .Invoke(this, new object[] { persistQuery, parameters });
        }

        public bool Delete(TEntity entity)
        {
            if (entity == null)
                return false;

            if (_handler.AutoGenerateCrudSql)
            {
                var sp = SpBuilder.BuildDeleteByIdSp(entity.SchemaName, _handler.IgnoreTablePrefixes, _handler, entity.PrimaryKeyInfo.columnDescription, entity.Id.ToString());
                _handler.ExecuteNonQuery<TEntity>(sp, null);
                return true;
            }

            var spName = SpBuilder.BuildDeleteByIdSp(entity.SchemaName, _handler.IgnoreTablePrefixes);
            _handler.ExecuteUniqueSp<TEntity>(new List<SqlParameter>
            {
                new SqlParameter(entity.PrimaryKeyInfo.columnDescription, entity.Id)
            }, spName);

            return true;
        }

        public TEntity RetreiveById(TId id)
        {
            if (_handler.AuditCrudEnabled)
            {
                
            }

            //get entity
            var entity = Activator.CreateInstance<TEntity>();
            entity.SetId(id);

            //get primary key
            var primaryKey = entity.EntityInfo.PrimaryKeyInfo.columnDescription;

            //select query
            var selectQuery = SpBuilder.BuildRetrieveByIdSp(entity.SchemaName, _handler, _handler.IgnoreTablePrefixes,
                entity.EntityInfo.PrimaryKeyInfo.columnDescription, Convert.ToInt64(entity.Id));

            //build params
            var param = _handler.AutoGenerateCrudSql ? null : new List<SqlParameter> { new SqlParameter("@" + primaryKey, entity.Id) };

            //exec unique
            var result = ExecuteUniqueQuery<TEntity>(selectQuery, param);
            return result;
        }

        /// <summary>
        /// Retreive All
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TEntity> RetreiveAll()
        {
            //get entity
            var entity = Activator.CreateInstance<TEntity>();

            //select query
            var selectQuery = SpBuilder.BuildRetrieveAllSp(entity.SchemaName, _handler.IgnoreTablePrefixes, _handler);

            //exec unique
            var data = ExecuteQuery<TEntity>(selectQuery, null);
            return data.OfType<TEntity>();
        }
        #endregion
    }
}
