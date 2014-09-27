using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.util.reflection;

namespace app.core.data.common.core.relation
{
    public class ColumnInfo
    {
        private string _columnDescription;
        public ColumnInfo ColumnDescription(string desc)
        {
            _columnDescription = desc;
            return this;
        }

        public bool HasMany
        {
            get
            {
                return ReflectionHelper.IsTypeCollection(_type);
            }
        }

        private long _maxLength;
        public ColumnInfo MaxLength(long maxLength)
        {
            _maxLength = maxLength;
            return this;
        }

        private Type _type;
        public ColumnInfo SetType(Type type)
        {
            _type = type;
            return this;
        }
    }
}
