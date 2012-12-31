using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Validation.Providers;
using Newtonsoft.Json;

using Geico.Applications.Data.PolicyDomain.Entities;
using RetrievePolicyWeb.Logging;


namespace RetrievePolicyWeb
{
    public static class WebApiConfig
    {
        /// <summary>
        /// The register.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public static void Register(HttpConfiguration config)
        {
            GlobalConfiguration.Configuration.Services.RemoveAll(
                typeof(System.Web.Http.Validation.ModelValidatorProvider),
                v => v is InvalidModelValidatorProvider);

            //config.Routes.MapHttpRoute(
            // name: "GetPolicy",
            // routeTemplate: "api/policy/policyterm",
            // defaults: new
            // {
            //     controller = "RetrievePolicy",
            //     action = "GetPolicy"
            // });

            config.MessageHandlers.Add(new LoggingHandler());

            // Override the default implementation of JSON media type formatter
            var index = config.Formatters.IndexOf(config.Formatters.JsonFormatter);
            config.Formatters[index] = new Formatter.TypedJsonMediaTypeFormatter(
                new Dictionary<Type, JsonSerializerSettings>()
                    {
                        { typeof(IPolicyContainer), InitJsonSerializerSettings() }
                    });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializerSettings"/> type.
        /// </summary>
        /// <returns>The <see cref="JsonSerializerSettings"/> object.</returns>
        private static JsonSerializerSettings InitJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.None,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };
            settings.Converters.Add(new PolicyJsonConverter());

            return settings;
        }
    }
}