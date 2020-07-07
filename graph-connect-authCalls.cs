using System;
//using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Identity.Client;

namespace pwsh_graph_connect
{
    public class ClientAppFactory
    {
        public IPublicClientApplication CreatePublicClient(string clientId, string tenantId)
        {
            IPublicClientApplication clientApp = PublicClientApplicationBuilder.Create(clientId)
                .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                .WithDefaultRedirectUri()
                .Build();

            return clientApp;
        }

        public IConfidentialClientApplication CreatePrivateClient(string clientId, string tenantId, X509Certificate2 clientCert)
        {
            IConfidentialClientApplication clientApp = ConfidentialClientApplicationBuilder.Create(clientId)
                        .WithCertificate(clientCert)
                        .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                        .WithTenantId(tenantId)
                        .Build();

            return clientApp;
        }
    }

    public class PublicAuthHelper
    {
        public PublicAuthHelper(IPublicClientApplication app)
        {
            App = app;
        }
        protected IPublicClientApplication App { get; private set; }

        public async Task<AuthenticationResult> AuthenticateWithDeviceCode(IEnumerable<string> scopes)
        {
            AuthenticationResult result = null;

            result = await App.AcquireTokenWithDeviceCode(
                scopes,
                deviceCodeCallback =>
                {
                    Console.WriteLine(deviceCodeCallback.Message);
                    return Task.FromResult(0);
                }
            ).ExecuteAsync();

            return result;
        }
    }

    public class PrivateAuthHelper
    {
        public PrivateAuthHelper(IConfidentialClientApplication app)
        {
            App = app;
        }
        protected IConfidentialClientApplication App { get; private set; }

        public async Task<AuthenticationResult> getToken(IEnumerable<string> scopes)
        {
            AuthenticationResult result;
            result = await App.AcquireTokenForClient(scopes)
                .ExecuteAsync();

            return result;
        }
    }
}