using System;
using System.Collections.Generic;
using app.core.data.common.core.relation;

namespace app.core.data.common.builder.contract
{
    public interface IEntity
    {
        TimeSpan ExecutionSpan { get; set; }

        Dictionary<string, ColumnInfo> MapColumns { get; set; }

        PrimaryKeyInfo PrimaryKeyInfo { get; set; }

        EntityColumnSummary EntityInfo { get; }

        string SchemaName { get; }

        bool IsNew { get; }

        void SetId(object id);

        long GetId();
    }
}
