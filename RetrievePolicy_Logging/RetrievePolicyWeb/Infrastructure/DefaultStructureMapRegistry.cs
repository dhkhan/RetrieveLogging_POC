using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Geico.Applications.Business.PolicyApi.AttendedTransactions.Repositories;

using StructureMap;

namespace RetrievePolicyWeb.Infrastructure
{
    /// <summary>
    /// StructureMap Bootstrapper.
    /// </summary>
    internal class DefaultStructureMapRegistry
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="DefaultStructureMapRegistry"/> class from being created.
        /// </summary>
        private DefaultStructureMapRegistry()
        {
        }

        /// <summary>
        /// Initializes a new structure map object factory.
        /// </summary>
        /// <returns>
        /// The initialized ioc container.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Wiring up StructureMap expressions.")]
        internal static IContainer Initialize()
        {
            ObjectFactory.Initialize(expression =>
            {
                // Reference IntegrationServices dependencies.
                expression.AddRegistry(new AssemblySpecificStructureMapRegistry());

            });

            return ObjectFactory.Container;
        }
    }
}