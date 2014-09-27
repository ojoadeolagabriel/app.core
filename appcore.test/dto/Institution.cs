using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.data.common.core;

namespace appcore.test.dto
{
    public class Institution : Entity<long, Institution>
    {
        public string Name { get; set; }
    }
}
