using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Geico.Applications.Business.PolicyApi.AttendedTransactions.Repositories.Managers;

namespace RetrievePolicyWeb.Infrastructure
{
    public class StructureMapRegistry : StructureMap.Configuration.DSL.Registry
    {

        public StructureMapRegistry()
        {
            this.For<IPolicyManager>().Use<PolicyManager>();
        }
    }
}