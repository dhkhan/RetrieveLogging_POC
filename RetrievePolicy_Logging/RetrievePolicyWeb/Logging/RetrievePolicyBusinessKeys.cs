using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetrievePolicyWeb.Logging
{
    public class RetrievePolicyBusinessKeys
    {
        public string PolicyNumber { get; set; }

        public string EffectiveDate { get; set; }

        public string CompanyCode { get; set; }

        public string LineOfBusiness { get; set; }

        public string RiskState { get; set; }

        public string Calling_System { get; set; }

        public string Activity_Source { get; set; }

        public string MessageID { get; set; }

    }
}