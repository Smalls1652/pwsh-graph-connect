using System;
//using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
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
                deviceCodeCallback => {
                    Console.WriteLine(deviceCodeCallback.Message);
                    return Task.FromResult(0);
                }
            ).ExecuteAsync();

            return result;
        }
    }
}