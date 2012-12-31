using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace RetrievePolicyWeb.Logging
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly string EVENT_CLASSIFICATION_SRVC_ACCESSED_REQ = "Service Accessed-Request";
        private readonly string EVENT_CLASSIFICATION_SRVC_ACCESSED_RESP = "Service Accessed-Response";

        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            // Log the request information
            LogRequestLoggingInfo(request);

            // Execute the request
            var response = base.SendAsync(request, cancellationToken);

            response.ContinueWith((responseMsg) =>
            {
                // Extract the response logging info then persist the information
                LogResponseLoggingInfo(responseMsg.Result);
            });

            return response;
        }

        private void LogRequestLoggingInfo(HttpRequestMessage request)
        {
            dynamic keys = new
            {
                HttpMethod = request.Method.Method,
                UriAccessed = request.RequestUri.AbsoluteUri,
                AbsolutePath = request.RequestUri.AbsolutePath,
                IpAddress = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "0.0.0.0",
                MessageType = "Request",
                Headers = ExtractMessageHeadersIntoLoggingInfo(request.Headers.ToList()),
                MessageId = (int)EventClassification.WebServiceRequest

            };

            var e = new RetrievePolicyBusinessEvent(EventClassification.WebServiceRequest.ToString()).WithKeys(new RetrievePolicyBusinessKeys
            {
                //ExceedClientId = "asdf",
                //PolicyNumber = "123"

            });


            //if (request.Content != null)
            //{
            //    request.Content.ReadAsByteArrayAsync()
            //        .ContinueWith((task) =>
            //        {
            //            //           keys.BodyContent = UTF8Encoding.UTF8.GetString(task.Result);
            //            e.Raise(EVENT_CLASSIFICATION_SRVC_ACCESSED_REQ, keys);

            //        });

            //    //return;
            //}

            e.Raise(EVENT_CLASSIFICATION_SRVC_ACCESSED_REQ, keys);
        }

        private string ExtractMessageHeadersIntoLoggingInfo(List<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            var headersString = new StringBuilder();

            headers.ForEach(h =>
            {
                // convert the header values into one long string from a series of IEnumerable<string> values so it looks for like a HTTP header
                var headerValues = new StringBuilder();

                if (h.Value != null)
                {
                    foreach (var hv in h.Value)
                    {
                        if (headerValues.Length > 0)
                        {
                            headerValues.Append(", ");
                        }
                        headerValues.Append(hv);
                    }
                }

                headersString.AppendFormat("{1}: {0}", h.Key, headerValues.ToString());
            });
            return headersString.ToString();
        }

        private void LogResponseLoggingInfo(HttpResponseMessage response)
        {
            //var info = new ApiLoggingInfo();
            dynamic keys = new
            {
                MessageType = "Response",
                HttpMethod = response.RequestMessage.Method.ToString(),
                ResponseStatusCode = response.StatusCode,
                ResponseStatusMessage = response.ReasonPhrase,
                UriAccessed = response.RequestMessage.RequestUri.AbsoluteUri,
                IpAddress = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "0.0.0.0",
                Headers = ExtractMessageHeadersIntoLoggingInfo(response.Headers.ToList()),
                MessageId = (int)EventClassification.WebServiceResponse
            };

            var e = new RetrievePolicyBusinessEvent(EventClassification.WebServiceRequest.ToString()).WithKeys(new RetrievePolicyBusinessKeys
            {
                //ExceedClientId = "asdf",
                //PolicyNumber = "123"

            });
            //ExtractMessageHeadersIntoLoggingInfo(info, response.Headers.ToList());

            //if (response.Content != null)
            //{
            //    response.Content.ReadAsByteArrayAsync()
            //        .ContinueWith(t =>
            //        {
            //            //var responseMsg = System.Text.UTF8Encoding.UTF8.GetString(t.Result);
            //            //info.BodyContent = responseMsg;
            //           });

            //    //return;
            //}

            e.Raise(EVENT_CLASSIFICATION_SRVC_ACCESSED_RESP, keys);
        }
    }
}