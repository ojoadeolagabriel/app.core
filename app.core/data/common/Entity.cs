using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace app.core.data.common
{
    public class Entity<TId>
    {
        public TId Id { get; private set; }
    }
}
