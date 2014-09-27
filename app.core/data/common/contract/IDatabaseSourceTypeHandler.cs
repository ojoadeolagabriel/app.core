using System;
using app.core.data.common.core;

namespace app.core.data.common.contract
{
    public interface IDatabaseSourceTypeHandler
    {
        string DatabaseUnit { get; set; }
        String ConnectionString { get; set; }
    }
}
