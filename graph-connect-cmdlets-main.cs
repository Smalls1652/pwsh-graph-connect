//using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
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
            ParameterSetName = "ManualEntry"
        )]
        [Parameter(
            ParameterSetName = "PrivateApp"
        )]
        public SwitchParameter PrivateApp;

        [Parameter(
            Position = 3,
            ParameterSetName = "ManualEntry"
        )]
        [Parameter(
            ParameterSetName = "PrivateApp"
        )]
        public X509Certificate2 AuthCert;

        [Parameter(
            Position = 4,
            ParameterSetName = "ManualEntry"
        )]
        [Parameter(
            ParameterSetName = "PrivateApp"
        )]
        public string[] Scopes = new [] { "https://graph.microsoft.com/.default" };

        [Parameter(
            Position = 5,
            ParameterSetName = "InputObject",
            ValueFromPipeline = true
        )]
        public SavedGraphApiConnection InputObject;

        private dynamic clientApp;
        private dynamic authHelper;
        private AuthenticationResult authContextResult;

        protected override void ProcessRecord()
        {
            IEnumerable<string> authScopes = Scopes;

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
            switch (PrivateApp.IsPresent)
            {
                case true:
                    clientApp = new ClientAppFactory().CreatePrivateClient(ClientId, TenantId, AuthCert);

                    authHelper = new PrivateAuthHelper(clientApp);

                    WriteVerbose("Starting authentication process.");
                    authContextResult = authHelper.getToken(authScopes).GetAwaiter().GetResult();
                    break;

                default:
                    clientApp = new ClientAppFactory().CreatePublicClient(ClientId, TenantId);

                    authHelper = new PublicAuthHelper(clientApp);

                    WriteVerbose("Starting authentication process.");
                    authContextResult = authHelper.AuthenticateWithDeviceCode(authScopes).GetAwaiter().GetResult();
                    break;
            }
        }

        protected override void EndProcessing()
        {
            WriteObject(authContextResult);
        }
    }
}
