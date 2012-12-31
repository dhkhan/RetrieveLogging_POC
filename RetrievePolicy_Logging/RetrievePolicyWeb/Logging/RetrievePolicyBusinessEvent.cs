using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Geico.Applications.Foundation.EventProcessing;

namespace RetrievePolicyWeb.Logging
{
    public class RetrievePolicyBusinessEvent : BusinessEvent<RetrievePolicyBusinessKeys>
    {
        public RetrievePolicyBusinessEvent(string eventClassification)
            : base(eventClassification)
        { }

        public override string ApplicationId
        {
            get { return "GetPolicy-WebAPI"; }
        }
    }
}