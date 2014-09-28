﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

        public IEntity ExecuteUniqueSp(IEntity entity, List<SqlParameter> param, string selectQuery)
        {
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
                    if (!item.Value.HasMany)
                    {
                        var columnName = item.Value._columnDescription;
                        var columnData = ReadColumn(reader, columnName);

                        var typedColumnData = Convert.ChangeType(columnData, item.Value._type);
                        entity.GetType().GetProperty(item.Key).SetValue(entity, typedColumnData, null);
                    }
                    else
                    {
                        var constructorInfo = typeof(List<>).MakeGenericType(item.Value._type).GetConstructor(Type.EmptyTypes);
                        if (constructorInfo != null)
                        {
                            var t = item.Value._type.GetGenericArguments()[0];
                            var childData = (IEntity)Activator.CreateInstance(t);
                            var columnId = ReadColumn(reader, childData.PrimaryKeyInfo.columnDescription);

                            var @params = new List<SqlParameter>();
                            var param = new SqlParameter(childData.PrimaryKeyInfo.columnDescription, columnId);
                            @params.Add(param);

                            var query = SpBuilder.BuildRetrieveByIdSp(childData.TableName);
                            var result = ExecuteUniqueSp(childData, @params, query);

                            entity.GetType().GetProperty(item.Key).SetValue(entity, result, null);
                        }
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
