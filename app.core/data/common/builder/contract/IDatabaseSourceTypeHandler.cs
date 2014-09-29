using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using app.core.data.common.contract;
using app.core.data.common.core;
using app.core.data.common.core.relation;

namespace app.core.data.common.builder.contract
{
    public interface IDatabaseSourceTypeHandler
    {
        string DatabaseUnit { get; set; }
        String ConnectionString { get; set; }
        string[] IgnoreTablePrefixes { get; set; }
        IEntity ExecuteUniqueSp(IEntity entity,List<SqlParameter> param, string selectQuery);
        IEntity ExecuteUniqueSp<TIEntity>(List<SqlParameter> param, string selectQuery);
        List<IEntity> ExecuteSp<TDto>(List<SqlParameter> param, string selectQuery);
    }
}
