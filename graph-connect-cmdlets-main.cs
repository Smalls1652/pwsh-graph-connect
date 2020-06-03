//using System;
using System.Collections.Generic;
using System.Management.Automation;
//using System.Management.Automation.Runspaces;
using Microsoft.Identity.Client;

namespace pwsh_graph_connect
{
    [Cmdlet(VerbsLifecycle.Start, "GraphApiConnection")]
    public class AddGraphApiConnection : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true
        )]
        [ValidateNotNullOrEmpty()]
        public string ClientId;

        [Parameter(
            Position = 1,
            Mandatory = true
        )]
        [ValidateNotNullOrEmpty()]
        public string TenantId;

        private AuthenticationResult authContextResult;

        protected override void ProcessRecord()
        {
            IEnumerable<string> authScopes = new [] {
                ".default"
            };

            WriteVerbose($"Creating client application for '{ClientId}'.");
            IPublicClientApplication clientApp = new ClientAppFactory().CreatePublicClient(ClientId, TenantId);

            PublicAuthHelper authHelper = new PublicAuthHelper(clientApp);

            WriteVerbose("Starting authentication process.");
            authContextResult = authHelper.AuthenticateWithDeviceCode(authScopes).GetAwaiter().GetResult();
        }

        protected override void EndProcessing()
        {
            WriteObject(authContextResult);
        }
    }
}
