using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace pwsh_graph_connect
{
    public class SavedGraphApiConnection
    {
        [JsonProperty("friendlyName")]
        public string FriendlyName { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("tenantId")]
        public string TenantId { get; set; }
    }

    public class saveHelper
    {
        private DirectoryInfo savedConnectionsDir = null;
        private FileInfo savedConnectionsFile = null;

        public List<SavedGraphApiConnection> GetCurrentSavedConnections()
        {
            EnsureConfigLocationExists();

            string savedConnectionsFileName = "savedConnections.json";

            string savedConnectionsFilePath = Path.Combine(savedConnectionsDir.FullName, savedConnectionsFileName);

            EnsureSavedConnectionsFileExists(savedConnectionsFilePath);

            StreamReader fileReader = new StreamReader(savedConnectionsFile.FullName);

            List<SavedGraphApiConnection> savedConnections = JsonConvert.DeserializeObject<List<SavedGraphApiConnection>>(fileReader.ReadToEnd());

            fileReader.Close();

            return savedConnections;
        }

        public List<SavedGraphApiConnection> AddToSavedConnections(SavedGraphApiConnection item)
        {
            List<SavedGraphApiConnection> savedConnections = GetCurrentSavedConnections();
            savedConnections.Add(item);

            WriteToSavedConnections(savedConnections);

            return savedConnections;
        }

        public List<SavedGraphApiConnection> RemoveFromSavedConnections(string friendlyName)
        {
            List<SavedGraphApiConnection> savedConnections = GetCurrentSavedConnections();
            
            List<SavedGraphApiConnection> modifiedList = new List<SavedGraphApiConnection>();

            foreach (SavedGraphApiConnection item in savedConnections)
            {
                if (item.FriendlyName != friendlyName)
                {
                    modifiedList.Add(item);
                }
            }

            WriteToSavedConnections(modifiedList);

            return modifiedList;
        }

        private void EnsureConfigLocationExists()
        {
            string appDataDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
            string dirName = ".pwsh-graph-connect";
            string dirPath = Path.Combine(appDataDir, dirName);

            switch ((Directory.Exists(dirPath)))
            {
                case true:
                    savedConnectionsDir = new DirectoryInfo(dirPath);
                    break;

                default:
                    savedConnectionsDir = Directory.CreateDirectory(dirPath);
                    break;
            }
        }

        private void EnsureSavedConnectionsFileExists(string filePath)
        {
            switch ((File.Exists(filePath)))
            {
                case false:
                    FileStream createdFile = File.Create(filePath);
                    createdFile.Close();

                    StreamWriter initConfigWriter = new StreamWriter(filePath);

                    List<SavedGraphApiConnection> defaultConfig = new List<SavedGraphApiConnection>();

                    string initConfig = JsonConvert.SerializeObject(defaultConfig);
                    initConfigWriter.Write(initConfig);

                    initConfigWriter.Close();
                    savedConnectionsFile = new FileInfo(filePath);
                    break;

                default:
                    savedConnectionsFile = new FileInfo(filePath);
                    break;
            }
        }

        private void WriteToSavedConnections(List<SavedGraphApiConnection> item)
        {
            StreamWriter configWriter = new StreamWriter(savedConnectionsFile.FullName);

            configWriter.Write(JsonConvert.SerializeObject(item));

            configWriter.Close();
        }
    }
}