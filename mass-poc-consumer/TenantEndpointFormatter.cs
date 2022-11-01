using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mass_poc_consumer
{
    internal class TenantEndpointFormatter : KebabCaseEndpointNameFormatter
    {
        public TenantEndpointFormatter(IConfiguration configuration)
            : base(configuration.GetValue<string>("instance_tenant"), false)
        {
        }
    }
}
