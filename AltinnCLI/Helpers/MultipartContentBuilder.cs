using Altinn.Platform.Storage.Interface.Models;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AltinnCLI.Helpers
{
    public class MultipartContentBuilder
    {
        private readonly MultipartFormDataContent _builder;

        public MultipartContentBuilder(Instance instanceTemplate)
        {
            _builder = new MultipartFormDataContent();
            if (instanceTemplate != null)
            {
                StringContent instanceContent = new(JsonSerializer.Serialize(instanceTemplate), Encoding.UTF8, "application/json");

                _builder.Add(instanceContent, "instance");
            }
        }

        public MultipartContentBuilder AddDataElement(string elementType, Stream stream, string contentType)
        {
            StreamContent streamContent = new(stream);
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

            _builder.Add(streamContent, elementType);

            return this;
        }

        public MultipartContentBuilder AddDataElement(string elementType, StringContent content)
        {
            _builder.Add(content, elementType);

            return this;
        }

        public MultipartFormDataContent Build()
        {
            return _builder;
        }
    }
}
