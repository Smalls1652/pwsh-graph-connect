using System.Collections.Generic;
using System.Management.Automation;

namespace pwsh_graph_connect
{
    [Cmdlet(VerbsCommon.Get, "GraphApiSavedConnections")]
    public class GetGraphApiSavedConnections : PSCmdlet
    {
        [Parameter(
            Position = 0
        )]
        public string FriendlyName;

        private saveHelper saveHelperObj = new saveHelper();

        protected override void ProcessRecord()
        {
            List<SavedGraphApiConnection> savedData = saveHelperObj.GetCurrentSavedConnections();

            foreach (SavedGraphApiConnection item in savedData)
            {
                switch (string.IsNullOrEmpty(FriendlyName))
                {
                    case false:
                        if (item.FriendlyName == FriendlyName)
                        {
                            WriteObject(item);
                        }
                        break;

                    default:
                        WriteObject(item);
                        break;
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Add, "GraphApiSavedConnection")]
    public class AddGraphApiSavedConnection : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true
        )]
        [ValidateNotNullOrEmpty()]
        public string FriendlyName;

        [Parameter(
            Position = 1,
            Mandatory = true
        )]
        [ValidateNotNullOrEmpty()]
        public string ClientId;

        [Parameter(
            Position = 2,
            Mandatory = true
        )]
        [ValidateNotNullOrEmpty()]
        public string TenantId;

        private saveHelper saveHelperObj = new saveHelper();

        protected override void ProcessRecord()
        {
            SavedGraphApiConnection itemToAdd = new SavedGraphApiConnection();
            itemToAdd.FriendlyName = FriendlyName;
            itemToAdd.ClientId = ClientId;
            itemToAdd.TenantId = TenantId;

            List<SavedGraphApiConnection> savedData = saveHelperObj.AddToSavedConnections(itemToAdd);

            foreach (SavedGraphApiConnection item in savedData)
            {
                WriteObject(item);
            }
        }
    }

    [Cmdlet(VerbsCommon.Remove, "GraphApiSavedConnection")]
    public class RemoveGraphApiSavedConnection : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true
        )]
        [ValidateNotNullOrEmpty()]
        public string FriendlyName;

        private saveHelper saveHelperObj = new saveHelper();
        private List<SavedGraphApiConnection> savedConnections;

        protected override void ProcessRecord()
        {
            savedConnections = saveHelperObj.RemoveFromSavedConnections(FriendlyName);
        }

        protected override void EndProcessing()
        {
            foreach (SavedGraphApiConnection item in savedConnections)
            {
                WriteObject(item);
            }
        }
    }
}