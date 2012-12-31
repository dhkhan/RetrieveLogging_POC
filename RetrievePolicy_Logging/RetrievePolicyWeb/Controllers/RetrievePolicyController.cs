using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Geico.Applications.Business.PolicyApi.AttendedTransactions.Repositories.Managers;
using Geico.Applications.Business.PolicyApi.AttendedTransactions.Repositories.Extensions;

using Geico.Applications.Business.PolicyApi.AttendedTransactions.Core.Model;

namespace RetrievePolicyWeb.Controllers
{
    public class RetrievePolicyController : ApiController
    {
        /// <summary>
        /// Variable for IPolicyManager.
        /// </summary>
        private readonly IPolicyManager policyManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyController"/> class.
        /// </summary>
        /// <param name="policyManager">The Policy Manager.</param>
        /// <param name="oasisPolicyIdManager">The OasisPolicyIdManager.</param>
        /// <param name="policyStatusManager">The policy status manager.</param>
        public RetrievePolicyController(IPolicyManager policyManager)
        {
            this.policyManager = policyManager;
        }

        public RetrievePolicyController()
        {
            this.policyManager = StructureMap.ObjectFactory.GetInstance<IPolicyManager>();
        }

        /// <summary>
        /// Gets policy by policy number or policy termId.
        /// </summary>
        /// <param name="policyNumber">Policy Number.</param>
        /// <param name="policyTermId">Policy TermId.</param>
        /// <returns>Returns policy.</returns>
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetPolicy(string policyNumber = null, int? policyTermId = null)
        {
            if (string.IsNullOrEmpty(policyNumber) && !policyTermId.HasValue)
            {


                this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Policy number or policy term id is required.");
                //throw new RequestValidationException(() => NotificationMessages.RequestArgumentException, messageParams: new { Arguments = "policyNumber, policyTermId", Detail = "Policy number or policy term id is required." });
            }

            if (!string.IsNullOrEmpty(policyNumber) && policyTermId.HasValue)
            {
                this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Policy number or policy term id are required, but a value should be provided for only one parameter.");
                //throw new RequestValidationException(() => NotificationMessages.RequestArgumentException, messageParams: new { Arguments = "policyNumber, policyTermId", Detail = "Policy number or policy term id are required, but a value should be provided for only one parameter." });
            }

            // If policy term id provided, then retrieve by policy term id - otherwise, retrieve by policy number
            PolicyTermInfo termInfo = policyTermId.HasValue ? this.GetPolicyByPolicyTermId(policyTermId.Value) : this.GetPolicyByPolicyNumber(policyNumber);

            return this.Request.CreateResponse(HttpStatusCode.OK, termInfo.ToContainer);
        }

        /// <summary>
        /// Retrieves policy by policy term id.
        /// </summary>
        /// <param name="policyTermId">The policy term id.</param>
        /// <returns>A <see cref="GetPolicyByPolicyNumber"/> object representing the requested policy term id.</returns>
        internal PolicyTermInfo GetPolicyByPolicyTermId(int policyTermId)
        {
            PolicyTermInfo policyTerm = this.policyManager.GetPolicyByTermId(policyTermId);

            // If policy term info is not found, throw error
            if (policyTerm == null)
            {
                this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Policy term is not found.");
                //throw new ResourceNotFoundException(() => NotificationMessages.PolicyNotFound, messageParams: new { ParameterName = "policy term id", ParameterValue = policyTermId.ToString(CultureInfo.InvariantCulture) });
            }

            return policyTerm;
        }

        /// <summary>
        /// Retrieves policy by policy number.
        /// </summary>
        /// <param name="policyNumber">The policy number.</param>
        /// <returns>A <see cref="GetPolicyByPolicyNumber"/> object representing the requested policy.</returns>
        internal PolicyTermInfo GetPolicyByPolicyNumber(string policyNumber)
        {
            if (!PolicyRule.IsValidPolicyNumber(policyNumber))
            {
                this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, string.Format("The provided policy number {0} is not valid. Policy number must be numerical and 10 digits in length.", policyNumber));
                //throw new RequestValidationException(() => NotificationMessages.RequestArgumentException, messageParams: new { Arguments = "policyNumber", Detail = string.Format("The provided policy number {0} is not valid. Policy number must be numerical and 10 digits in length.", policyNumber) });
            }

            PolicyTermInfo policyTerm = this.policyManager.GetPolicyByNumber(policyNumber);

            if (policyTerm == null)
            {
                this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Policy Not found.");
                //throw new ResourceNotFoundException(() => NotificationMessages.PolicyNotFound, messageParams: new { ParameterName = "policy number", ParameterValue = policyNumber });                
            }

            return policyTerm;
        }
    }
}
