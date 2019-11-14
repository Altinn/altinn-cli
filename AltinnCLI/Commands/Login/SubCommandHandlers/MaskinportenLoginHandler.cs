using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace AltinnCLI.Commands.Login.SubCommandHandlers
{
    public class MaskinportenLoginHandler : SubCommandHandlerBase, ISubCommandHandler, IHelp
    {       
        private MaskinPortenClientWrapper ClientWrapper = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetDocumentHandler" /> class.
        /// </summary>
        /// <param name="logger">Reference to the common logger that the application shall used to log log info and error information
        public MaskinportenLoginHandler(ILogger<MaskinportenLoginHandler> logger) : base(logger)
        {

            if (ApplicationManager.ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>())
            {
                ClientWrapper = new MaskinPortenClientWrapper(_logger);
            }
            else
            {
                ClientWrapper = new MaskinPortenClientWrapper(_logger);
            }
        }


        /// <summary>
        /// Gets the name of of the command
        /// </summary>
        public string Name
        {
            get
            {
                return "Maskinporten";
            }
        }
        /// <summary>
        /// Gets the name of the CommandProvider that uses this command
        /// </summary>
        public string CommandProvider
        {
            get
            {
                return "Login";
            }
        }

        /// <summary>
        /// Gets the description of the command handler that is used by the Help function
        /// </summary>
        public string Description
        {
            get
            {
                return "Logs in";
            }
        }

        /// <summary>
        /// Gets how the command can be specified to get documents, is used by the Help function
        /// </summary>
        public string Usage
        {
            get
            {
                string usage = $"\n" +
                $@"Login clentId=<client guid id> cert=<c:\cerificat\fileName> \n"+
                $"\n" +
                $" Required parameters for the login command \n";

                foreach (IOption opt in SelectableCliOptions)
                {
                    usage += $"\t{opt.Name}\t\t {opt.Description} \n";
                }


                return usage;
            }
        }
        public string GetHelp()
        {
            throw new NotImplementedException();
        }

        public bool Run()
        {
            if (IsValid)
            {
                IOption fileNameOption = SelectableCliOptions.FirstOrDefault(x => string.Equals(x.Name, "file", StringComparison.OrdinalIgnoreCase));

                FileStream stream = new FileStream(fileNameOption.Value, FileMode.Open);
                MemoryStream memstr = new MemoryStream(new byte[stream.Length]);
                stream.CopyTo(memstr);
                memstr.Position = 0;
                stream.Close();

                jwttoken

 //               InstanceResponseMessage responsMessage = ClientWrapper.UploadDataElement(SelectableCliOptions, memstr, fileNameOption.Value);
            }

            return true;

        }

        public string GetAccessToken(string assertion, out bool isError)
        {
            var formContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
                new KeyValuePair<string, string>("assertion", assertion),
            });

            var client = new HttpClient();
            var response = client.PostAsync(_tokenEndpoint, formContent).Result;
            isError = !response.IsSuccessStatusCode;
            return response.Content.ReadAsStringAsync().Result;
        }

        public string GetJwtAssertion()
        {
            var dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);

            var cert = GetCertificateFromKeyStore(_certificateThumbPrint, StoreName.My, StoreLocation.LocalMachine);
            var securityKey = new X509SecurityKey(cert);
            var header = new JwtHeader(new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256))
            {
                {"x5c", new List<string>() {Convert.ToBase64String(cert.GetRawCertData())}}
            };
            header.Remove("typ");
            header.Remove("kid");

            var payload = new JwtPayload
            {
                { "aud", _audience },
                { "resource", _resource },
                { "scope", _scopes },
                { "iss", _issuer },
                { "exp", dateTimeOffset.ToUnixTimeSeconds() + _tokenTtl },
                { "iat", dateTimeOffset.ToUnixTimeSeconds() },
                { "jti", Guid.NewGuid().ToString() },
            };

            var securityToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(securityToken);
        }
    }
}
