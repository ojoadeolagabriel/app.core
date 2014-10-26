using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using app.core.data.common.builder;
using app.core.data.common.builder.contract;

namespace app.core.data.common.handler
{
    public class OracleHandler : IDatabaseSourceTypeHandler
    {
        public class MsSqlHandlerSpTemplates
        {
            public const string SelectById = "select * from [dbo].[{0}] where {1} = '{2}'";
            public const string SelectAll = "select * from [dbo].[{0}] with (nolock)";
            public const string Update = "update {0} set {1} where {2} = '{3}'";
            public const string Insert = "insert into {0} ({1}) values ({2}) select SCOPE_IDENTITY()";
            public const string Delete = "delete from {0} where {1} = '{2}'";
        }

        public List<string> SpListCache { get; set; }
        public string DatabaseUnit { get; set; }
        public string ConnectionString { get; set; }
        public string[] IgnoreTablePrefixes { get; set; }
        public bool AutoGenerateCrudSql { get; set; }
        public bool ForceUseOfExistingSp { get; set; }
        public bool IgnoreForeignRelationship { get; set; }
        public bool AuditCrudEnabled { get; set; }

        public IEntity ExecuteUniqueSp<TIEntity>(List<SqlParameter> param, string selectQuery)
        {
            var timeSpan = new Stopwatch();
            timeSpan.Start();

            var entity = (IEntity)Activator.CreateInstance(typeof(TIEntity));
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var command = AutoGenerateCrudSql && param == null ?
                    new SqlCommand(selectQuery, connection) { CommandType = CommandType.Text } :
                    new SqlCommand(selectQuery, connection) { CommandType = CommandType.StoredProcedure };

                if (!AutoGenerateCrudSql || param != null)
                    if (param.Count > 0)
                        command.Parameters.AddRange(param.ToArray());

                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ReadColumns(reader, entity);
                        entity.ExecutionSpan = timeSpan.Elapsed;
                        timeSpan.Stop();

                        return entity;
                    }
                }
            }

            return null;
        }

        public List<IEntity> ExecuteSp<TIEntity>(List<SqlParameter> param, string selectQuery)
        {
            var entity = (IEntity)Activator.CreateInstance(typeof(TIEntity));
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var command = AutoGenerateCrudSql && param == null ?
                    new SqlCommand(selectQuery, connection) { CommandType = CommandType.Text } :
                    new SqlCommand(selectQuery, connection) { CommandType = CommandType.StoredProcedure };

                if (!AutoGenerateCrudSql || param != null)
                    if (param.Count > 0)
                        command.Parameters.AddRange(param.ToArray());

                var reader = command.ExecuteReader();

                if (!reader.HasRows) return null;

                var coll = new List<IEntity>();
                while (reader.Read())
                {
                    var timeSpan = new Stopwatch();
                    timeSpan.Start();

                    var copyEntity = (IEntity)Activator.CreateInstance(entity.GetType());
                    ReadColumns(reader, copyEntity);

                    entity.ExecutionSpan = timeSpan.Elapsed;
                    timeSpan.Stop();

                    coll.Add(copyEntity);
                }
                return coll;
            }
        }

        /// <summary>
        ///     Execute Non Query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ExecuteNonQuery<T>(string selectQuery, List<SqlParameter> param)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = AutoGenerateCrudSql ?
                     new SqlCommand(selectQuery, connection) { CommandType = CommandType.Text } :
                     new SqlCommand(selectQuery, connection) { CommandType = CommandType.StoredProcedure };

                if (!AutoGenerateCrudSql || param.Count > 0)
                    if (param.Count > 0)
                        command.Parameters.AddRange(param.ToArray());
                var data = command.ExecuteScalar();

                return (T)Convert.ChangeType(data, typeof(T));
            }
        }

        public string AutoGenerateSelectByIdQuery<TId>(string entity, string primaryKeyColumn, TId idValue)
        {
            var query = string.Format(MsSqlHandler.MsSqlHandlerSpTemplates.SelectById, entity, primaryKeyColumn, idValue);
            return query;
        }

        public string AutoGenerateSelectAllQuery(string entity)
        {
            var query = string.Format(MsSqlHandler.MsSqlHandlerSpTemplates.SelectAll, entity);
            return query;
        }

        public string AutoGenerateUpdateQuery(string schema, IDictionary<string, object> data, string primaryKeyColumnName, string primaryKeyColumnValue)
        {
            var setPart = data.Aggregate("", (current, item) => current + string.Format("{0} = '{1}',", item.Key, ReadPart(item)));

            setPart = setPart.Remove(setPart.Length - 1, 1);

            var query = string.Format(MsSqlHandler.MsSqlHandlerSpTemplates.Update, schema, setPart,
                primaryKeyColumnName, primaryKeyColumnValue);

            return query;
        }

        public string AutoGenerateDeleteQuery(string schema, string primaryKeyColumnName, string primaryKeyColumnValue)
        {
            var query = string.Format(MsSqlHandler.MsSqlHandlerSpTemplates.Delete, schema, primaryKeyColumnName, primaryKeyColumnValue);
            return query;
        }

        public string AutoGenerateCreateQuery(string schema, IDictionary<string, object> data)
        {
            var setPart = data.Aggregate("", (current, item) => current + string.Format("{0},", item.Key));

            setPart = setPart.Remove(setPart.Length - 1, 1);

            var valuePart = data.Aggregate("", (current, item) => current + string.Format("'{0}',", ReadPart(item)));
            valuePart = valuePart.Remove(valuePart.Length - 1, 1);

            var query = string.Format(MsSqlHandler.MsSqlHandlerSpTemplates.Insert, schema, setPart, valuePart);
            return query;
        }

        private static string ReadPart(KeyValuePair<string, object> item)
        {
            if (item.Value == null)
                return "";
            return (item.Value is IEntity) ? ((IEntity)item.Value).GetId().ToString(CultureInfo.InvariantCulture) : item.Value.ToString().Replace("'", "");
        }

        /// <summary>
        ///     Read Columns
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="entity"></param>
        private void ReadColumns(IDataRecord reader, IEntity entity)
        {
            var columns = entity.MapColumns;

            var primaryKeyColumnName = entity.PrimaryKeyInfo.columnDescription;
            var primaryKeyColumnData = ReadColumn(reader, primaryKeyColumnName);

            try
            {
                var primaryKeyTypedColumnData = Convert.ChangeType(primaryKeyColumnData, entity.PrimaryKeyInfo.ColumnType);
                entity.SetId(primaryKeyTypedColumnData);
            }
            catch(Exception exc)
            {

            }

            foreach (var item in columns)
            {
                try
                {
                    //process single map
                    if (!item.Value.IsForeign)
                    {
                        var columnName = item.Value._columnDescription;
                        var columnData = ReadColumn(reader, columnName);

                        var typedColumnData = Convert.ChangeType(columnData, item.Value._type);
                        entity.GetType().GetProperty(item.Key).SetValue(entity, typedColumnData, null);
                    }
                    else
                    {
                        if (IgnoreForeignRelationship)
                            continue;

                        var childType = item.Value._type;
                        var childData = (IEntity)Activator.CreateInstance(childType);
                        var columnId = ReadColumn(reader, childData.PrimaryKeyInfo.columnDescription);

                        var @params = new List<SqlParameter>();
                        if (!AutoGenerateCrudSql)
                        {
                            var param = new SqlParameter(childData.PrimaryKeyInfo.columnDescription, columnId);
                            @params.Add(param);
                        }
                        else
                        {
                            @params = null;
                        }

                        var query = SpBuilder.BuildRetrieveByIdSp(childData.SchemaName, this, IgnoreTablePrefixes, childData.PrimaryKeyInfo.columnDescription, Convert.ToInt64(columnId));

                        var result = GetType()
                            .GetMethod("ExecuteUniqueSp")
                            .MakeGenericMethod(item.Value._type)
                            .Invoke(this, new object[] { @params, query });

                        entity.GetType().GetProperty(item.Key).SetValue(entity, result, null);
                    }
                }
                catch (Exception exc)
                {
                }
            }
        }

        /// <summary>
        /// Read Column.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private static object ReadColumn(IDataRecord reader, string columnName)
        {
            try
            {
                return reader[columnName];
            }
            catch
            {
                return "";
            }
        }
    }
}
