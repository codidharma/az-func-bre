using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Az.Serverless.Bre.Func01.Configuration
{
    public class BlobRulesStoreConfiguration
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }
}
