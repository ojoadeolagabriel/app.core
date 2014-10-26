using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace app.core.data.audit
{
    public class AuditData
    {
        public XmlElement OldData { get; set; }
        public XmlElement NewData { get; set; }
    }
}
