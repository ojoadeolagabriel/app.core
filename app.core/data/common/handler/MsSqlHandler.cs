using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using app.core.data.common.builder;
using app.core.data.common.builder.contract;
using app.core.data.common.contract;
using app.core.data.common.core;
using app.core.data.common.core.relation;
using app.core.util.reflection;

namespace app.core.data.common.handler
{
    /// <summary>
    /// 
    /// </summary>
    public class MsSqlHandler : IDatabaseSourceTypeHandler
    {
        public string DatabaseUnit { get; set; }
        public string ConnectionString { get; set; }
        public string[] IgnoreTablePrefixes { get; set; }

        public IEntity ExecuteUniqueSp<TIEntity>(List<SqlParameter> param, string selectQuery)
        {
            var entity = (IEntity)Activator.CreateInstance(typeof(TIEntity));
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(selectQuery, connection) { CommandType = CommandType.StoredProcedure };
                if (param.Count > 0)
                    command.Parameters.AddRange(param.ToArray());

                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ReadColumns(reader, entity);
                        return entity;
                    }
                }
            }

            return null;
        }

        public List<IEntity> ExecuteSp<TDto>(List<SqlParameter> param, string selectQuery)
        {
            var entity = (IEntity)Activator.CreateInstance(typeof(TDto));
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(selectQuery, connection) { CommandType = CommandType.StoredProcedure };
                if (param != null && param.Count > 0)
                    command.Parameters.AddRange(param.ToArray());

                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    var coll = new List<IEntity>();
                    while (reader.Read())
                    {
                        var copyEntity = (IEntity)Activator.CreateInstance(entity.GetType());
                        ReadColumns(reader, copyEntity);
                        coll.Add(copyEntity);
                    }
                    return coll;
                }
            }

            return null;
        }

        /// <summary>
        /// Read Columns
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="entity"></param>
        private void ReadColumns(SqlDataReader reader, IEntity entity)
        {
            var columns = entity.MapColumns;
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
                        var childType = item.Value._type;
                        var childData = (IEntity)Activator.CreateInstance(childType);
                        var columnId = ReadColumn(reader, childData.PrimaryKeyInfo.columnDescription);

                        var @params = new List<SqlParameter>();
                        var param = new SqlParameter(childData.PrimaryKeyInfo.columnDescription, columnId);
                        @params.Add(param);

                        var query = SpBuilder.BuildRetrieveByIdSp(childData.TableName);

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

        private object ReadColumn(SqlDataReader reader, string columnName)
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
