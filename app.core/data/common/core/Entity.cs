using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using app.core.data.common.contract;
using app.core.data.common.core.relation;
using app.core.util.reflection;
using Core.Buffer;

namespace app.core.data.common.core
{
    /// <summary>
    /// Entity Class
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class Entity<TId, TEntity> : IEntity
    {
        public Dictionary<string, ColumnInfo> MapColumns = new Dictionary<string, ColumnInfo>();

        public PrimaryKeyInfo PrimaryKeyInfo { get; set; }

        public EntityColumnSummary EntityInfo
        {
            get { return new EntityColumnSummary { MapColumns = MapColumns, PrimaryKeyInfo = PrimaryKeyInfo }; }
        }

        public string TableName { get; set; }
        public void OverrideTablename(string schema)
        {
            TableName = schema;
        }

        public TId Id { get; private set; }

        protected void SetId(TId id)
        {
            Id = id;
        }

        public String SchemaName
        {
            get
            {
                return null;
            }
        }

        public PrimaryKeyInfo PrimaryKey<T>(Expression<Func<TEntity, T>> expression)
        {
            var member = ReflectionHelper<TEntity>.GetMember(expression);
            if (PrimaryKeyInfo == null)
                PrimaryKeyInfo = new PrimaryKeyInfo();

            PrimaryKeyInfo.ColumnDescription(member.Name);
            return PrimaryKeyInfo;
        }

        public ColumnInfo Map<T>(Expression<Func<TEntity, T>> expression)
        {
            var member = ReflectionHelper<TEntity>.GetMember(expression);
            if (!MapColumns.ContainsKey(member.Name))
                MapColumns.Add(member.Name, new ColumnInfo());

            var mapColumn = MapColumns.First(c => c.Key == member.Name).Value;

            mapColumn.SetType(((PropertyInfo)member).PropertyType);
            return mapColumn;
        }

        public ColumnInfo HasMany<T>(Expression<Func<TEntity, T>> expression)
        {
            var member = ReflectionHelper<TEntity>.GetMember(expression);
            if (!MapColumns.ContainsKey(member.Name))
                MapColumns.Add(member.Name, new ColumnInfo());

            var mapColumn = MapColumns.First(c => c.Key == member.Name).Value;
            mapColumn.SetType(((PropertyInfo)member).PropertyType);
            return mapColumn;
        }
    }
}
