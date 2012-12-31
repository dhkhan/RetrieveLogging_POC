using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Geico.Applications.Data.PolicyDomain.Entities;
using Newtonsoft.Json;

namespace RetrievePolicyWeb.Formatter
{
    /// <summary>
    /// Custom JsonMediaTypeFormatter that provides the ability to specify custom serializer settings for object types.  
    /// </summary>
    public class TypedJsonMediaTypeFormatter : JsonMediaTypeFormatter
    {
        /// <summary>
        /// Registered settings.
        /// </summary>
        private readonly Dictionary<Type, JsonSerializerSettings> registeredSettings =
            new Dictionary<Type, JsonSerializerSettings>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedJsonMediaTypeFormatter"/> class.
        /// </summary>
        public TypedJsonMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedJsonMediaTypeFormatter"/> class.
        /// </summary>
        /// <param name="settings">A dictionary of the registered settings.</param>
        public TypedJsonMediaTypeFormatter(Dictionary<Type, JsonSerializerSettings> settings)
            : this()
        {
            foreach (var jsonSettings in settings)
            {
                this.registeredSettings.Add(jsonSettings.Key, jsonSettings.Value);
            }
        }

        /// <summary>
        /// Gets the RegisteredSettings.
        /// </summary>
        /// <value>The dictionary of registered settings.</value>
        public Dictionary<Type, JsonSerializerSettings> RegisteredSettings
        {
            get { return this.registeredSettings; }
        }

        /// <summary>
        /// Determines whether this <see cref="T:System.Net.Http.Formatting.JsonMediaTypeFormatter"/> can read objects of the specified <paramref name="type"/>.
        /// </summary>
        /// <returns>
        /// true if objects of this <paramref name="type"/> can be read, otherwise false.
        /// </returns>
        /// <param name="type">The type of object that will be read.</param>
        public override bool CanReadType(Type type)
        {
            return true;
        }

        /// <summary>
        /// Determines whether this <see cref="T:System.Net.Http.Formatting.JsonMediaTypeFormatter"/> can write objects of the specified <paramref name="type"/>.
        /// </summary>
        /// <returns>
        /// true if objects of this <paramref name="type"/> can be written, otherwise false.
        /// </returns>
        /// <param name="type">The type of object that will be written.</param>
        public override bool CanWriteType(Type type)
        {
            return true;
        }

        /// <summary>
        /// Reads an object of the specified <paramref name="type"/> from the specified <paramref name="readStream"/>. This method is called during deserialization.
        /// </summary>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.
        /// </returns>
        /// <param name="type">The type of object to read.</param>
        /// <param name="readStream">Thestream from which to read.</param>
        /// <param name="content">The content being written.</param>
        /// <param name="formatterLogger">The <see cref="T:System.Net.Http.Formatting.IFormatterLogger"/> to log events to.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "It is acceptable for these types to be disposed more than once.")]
        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, System.Net.Http.HttpContent content, IFormatterLogger formatterLogger)
        {
            //// Check if custom settings available
            JsonSerializerSettings serializerSettings = this.registeredSettings.ContainsKey(type)
                                          ? this.registeredSettings[type]
                                          : CreateDefaultSerializerSettings();

            var serializer = JsonSerializer.Create(serializerSettings);

            //// Create task reading the content
            return Task.Factory.StartNew(
                () =>
                {
                    using (var streamReader = new StreamReader(readStream))
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        return serializer.Deserialize(jsonTextReader, type);
                    }
                });
        }

        /// <summary>
        /// Writes an object of the specified <paramref name="type"/> to the specified <paramref name="writeStream"/>. This method is called during serialization.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task"/> that will write the value to the stream.
        /// </returns>
        /// <param name="type">The type of object to write.</param>
        /// <param name="value">The object to write.</param>
        /// <param name="writeStream">The <see cref="T:System.IO.Stream"/> to which to write.</param>
        /// <param name="content">The <see cref="T:System.Net.Http.HttpContent"/> where the content is being written.</param>
        /// <param name="transportContext">The <see cref="T:System.Net.TransportContext"/>.</param>
        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, TransportContext transportContext)
        {
            //// Check if custom settings available
            JsonSerializerSettings serializerSettings = this.registeredSettings.ContainsKey(type)
                                                            ? this.registeredSettings[type]
                                                            : CreateDefaultSerializerSettings();

            ////Create a serializer
            var serializer = JsonSerializer.Create(serializerSettings);

            //// Create task reading the content
            return Task.Factory.StartNew(() =>
                {
                    using (var jsonTextWriter = new JsonTextWriter(new StreamWriter(writeStream, new UTF8Encoding(false, true))) { CloseOutput = false })
                    {
                        serializer.Serialize(jsonTextWriter, value);
                        jsonTextWriter.Flush();
                    }
                });
        }
    }
}
