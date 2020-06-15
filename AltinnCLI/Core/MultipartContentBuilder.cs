using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using Altinn.Platform.Storage.Interface.Models;

using Newtonsoft.Json;

namespace AltinnCLI.Core
{
    public class MultipartContentBuilder
    {
        private readonly MultipartFormDataContent _builder;

        public MultipartContentBuilder(Instance instanceTemplate)
        {
            _builder = new MultipartFormDataContent();
            if (instanceTemplate != null)
            {
                StringContent instanceContent = new StringContent(JsonConvert.SerializeObject(instanceTemplate), Encoding.UTF8, "application/json");

                _builder.Add(instanceContent, "instance");
            }
        }

        public MultipartContentBuilder AddDataElement(string elementType, Stream stream, string contentType)
        {
            StreamContent streamContent = new StreamContent(stream);
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
