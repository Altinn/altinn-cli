using AltinnCLI.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AltinnCLI.Commands.Login.SubCommandHandlers
{
    public class MaskinportenLoginHandler : SubCommandHandlerBase, ISubCommandHandler, IHelp
    {       
        private MaskinportenClientWrapper ClientWrapper = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetDocumentHandler" /> class.
        /// </summary>
        /// <param name="logger">Reference to the common logger that the application shall used to log log info and error information
        public MaskinportenLoginHandler(ILogger<MaskinportenLoginHandler> logger) : base(logger)
        {

            if (ApplicationManager.ApplicationConfiguration.GetSection("UseLiveClient").Get<bool>())
            {
                ClientWrapper = new MaskinportenClientWrapper(_logger);
            }
            else
            {
                ClientWrapper = new MaskinportenClientWrapper(_logger);
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
                $@"Login clientId=<client guid id> thumbprint=<c:\cerificat\fileName> \n" +
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
            FindAllCeriticates();
            if (IsValid)
            {
                IOption fileNameOption = SelectableCliOptions.FirstOrDefault(x => string.Equals(x.Name, "file", StringComparison.OrdinalIgnoreCase));

                string jwtAssertion = GetJwtAssertion();


                if (!string.IsNullOrEmpty(jwtAssertion))
                {
                    FormUrlEncodedContent content = GetUrlEncodedContent(jwtAssertion);
                    string token = ClientWrapper.PostToken(content);
                }

                return true;
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
            string _certificateThumbPrint = (string)GetOptionValue("thumbprint");

            var cert = GetCertificateFromKeyStore(_certificateThumbPrint, StoreName.My, StoreLocation.CurrentUser);

            var securityKey = new X509SecurityKey(cert);
            var header = new JwtHeader(new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256))
            {
                {"x5c", new List<string>() {Convert.ToBase64String(cert.GetRawCertData())}}
            };
            header.Remove("typ");
            header.Remove("kid");

            var payload = new JwtPayload();
            //{
            //    { "aud", _audience },
            //    { "resource", _resource },
            //    { "scope", _scopes },
            //    { "iss", _issuer },
            //    { "exp", dateTimeOffset.ToUnixTimeSeconds() + _tokenTtl },
            //    { "iat", dateTimeOffset.ToUnixTimeSeconds() },
            //    { "jti", Guid.NewGuid().ToString() },
            //};

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

        private void FindAllCeriticates()
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser); // StoreLocation.CurrentUser); //StoreLocation.LocalMachine fails too
            store.Open(OpenFlags.ReadOnly);
            var certificates = store.Certificates;
            foreach (var certificate in certificates)
            {
                var friendlyName = certificate.FriendlyName;
                Console.WriteLine(friendlyName);
            }
        }
}
}
