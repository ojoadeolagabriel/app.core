using System.Collections.Generic;
using app.core.data.common.core.relation;

namespace app.core.data.common.builder.contract
{
    public interface IEntity
    {
        Dictionary<string, ColumnInfo> MapColumns { get; set; }

        PrimaryKeyInfo PrimaryKeyInfo { get; set; }

        EntityColumnSummary EntityInfo { get; }

        string TableName { get; }

        bool IsNew { get; }
    }
}
