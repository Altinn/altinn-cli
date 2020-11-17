using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

using AltinnCLI.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AltinnCLI.Commands.Login.SubCommandHandlers
{
    public class MaskinportenLoginHandler : SubCommandHandlerBase, ISubCommandHandler, IHelp
    {       
        private readonly MaskinportenClientWrapper _clientWrapper;
        private readonly IAutorizationClientWrapper _authorizationClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaskinportenLoginHandler" /> class.
        /// </summary>
        /// <param name="logger">Reference to the common logger that the application shall used to log log info and error information
        public MaskinportenLoginHandler(ILogger<MaskinportenLoginHandler> logger) : base(logger)
        {
            if (ApplicationManager.ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>())
            {
                _clientWrapper = new MaskinportenClientWrapper(_logger);
                _authorizationClient = new AuthorizationClientWrapper(_logger);
            }
            else
            {
                _clientWrapper = new MaskinportenClientWrapper(_logger);
                _authorizationClient = new AuthorizationClientFileWrapper();
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
                return "Logs in to the Marskinporten to be authenticated for accessing the Storage";
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
                $@"Login clientId=<client guid id> thumbprint=<thumbprint id> \n" +
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
            return Name;
        }

        public bool Run()
        {
            if (IsValid)
            {
                string jwtAssertion = GetJwtAssertion();

                if (!string.IsNullOrEmpty(jwtAssertion))
                {
                    FormUrlEncodedContent content = GetUrlEncodedContent(jwtAssertion);
                    if (_clientWrapper.PostToken(content, out string token))
                    {
                        if (!string.IsNullOrEmpty(token))
                        {
                            var accessTokenObject = JsonConvert.DeserializeObject<JObject>(token);

                            bool test = Convert.ToBoolean(GetOption("test").Value);
                            token = _authorizationClient.ConvertToken(accessTokenObject.GetValue("access_token").ToString(), test).GetAwaiter().GetResult();

                            ApplicationManager.IsLoggedIn = true;
                            ApplicationManager.MaskinportenToken = token;

                            _logger.LogInformation("Sucessfully validated against Maskinporten");
                            _logger.LogInformation($@"Altinn Security Token: {token}");

                            return true;
                        }
                    }

                    return false;
                }
            }

            return false;
        }

        public FormUrlEncodedContent GetUrlEncodedContent(string assertion)
        {
            FormUrlEncodedContent formContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
                new KeyValuePair<string, string>("assertion", assertion),
            });

            return formContent;
        }

        public string GetJwtAssertion()
        {
            var dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);
            string certificateThumbPrint = (string)GetOptionValue("thumbprint");
            Guid clientId = (Guid)GetOptionValue("clientid");

            var cert = GetCertificateFromKeyStore(certificateThumbPrint, StoreName.My, StoreLocation.CurrentUser);

            var securityKey = new X509SecurityKey(cert);
            var header = new JwtHeader(new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256))
            {
                {"x5c", new List<string>() {Convert.ToBase64String(cert.GetRawCertData())}}
            };
            header.Remove("typ");
            header.Remove("kid");

            var payload = new JwtPayload
            {
                { "aud", "https://ver2.maskinporten.no/" },
                { "resource", "https://tt02.altinn.no/maskinporten-api/" },
                { "scope", "altinn:serviceowner/instances.read altinn:serviceowner/instances.write" },
                { "iss",  clientId},
                { "exp", dateTimeOffset.ToUnixTimeSeconds() + 10 },
                { "iat", dateTimeOffset.ToUnixTimeSeconds() },
                { "jti", Guid.NewGuid().ToString() },
            };

            var securityToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(securityToken);
        }

        private static X509Certificate2 GetCertificateFromKeyStore(string thumbprint, StoreName storeName, StoreLocation storeLocation, bool onlyValid = false)
        {
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);
            var certCollection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, onlyValid);
            var enumerator = certCollection.GetEnumerator();
            X509Certificate2 cert = null;
            while (enumerator.MoveNext())
            {
                cert = enumerator.Current;
            }

            return cert;
        }

        public override bool Validate()
        {
            if (base.Validate())
            {
                if (GetOption("clientId").IsAssigned && GetOption("thumbprint").IsAssigned)
                {
                    return true;
                }

                ErrorMessage = ($"Command failed, missing command parameters, must specify clientid and thumbprint");

                return false;
            }

            return false;
        }
    }
}
