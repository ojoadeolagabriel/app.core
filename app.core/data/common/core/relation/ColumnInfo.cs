using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.util.reflection;

namespace app.core.data.common.core.relation
{
    public class ColumnInfo
    {
        public string _columnDescription;
        public ColumnInfo ColumnDescription(string desc)
        {
            _columnDescription = desc;
            return this;
        }

        public bool IsForeign { get; set; }

        private long _maxLength;
        public ColumnInfo MaxLength(long maxLength)
        {
            _maxLength = maxLength;
            return this;
        }

        public Type _type;
        public ColumnInfo SetType(Type type)
        {
            _type = type;
            return this;
        }

        private string _validationRegex;
        public ColumnInfo ValidationRegex(string validationString)
        {
            _validationRegex = validationString;
            return this;
        }
    }
}
