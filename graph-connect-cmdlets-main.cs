//using System;
using System.Collections.Generic;
using System.Management.Automation;
//using System.Management.Automation.Runspaces;
using Microsoft.Identity.Client;

namespace pwsh_graph_connect
{
    [Cmdlet(VerbsLifecycle.Start, "GraphApiConnection")]
    public class StartGraphApiConnection : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ParameterSetName = "ManualEntry"
        )]
        [ValidateNotNullOrEmpty()]
        public string ClientId;

        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = "ManualEntry"
        )]
        [ValidateNotNullOrEmpty()]
        public string TenantId;

        [Parameter(
            Position = 2,
            ParameterSetName = "InputObject",
            ValueFromPipeline = true
        )]
        public SavedGraphApiConnection InputObject;

        private AuthenticationResult authContextResult;

        protected override void ProcessRecord()
        {
            IEnumerable<string> authScopes = new [] {
                ".default"
            };

            switch (ParameterSetName)
            {
                case "InputObject":
                    ClientId = InputObject.ClientId;
                    TenantId = InputObject.TenantId;
                    break;

                default:
                    break;
            }

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
