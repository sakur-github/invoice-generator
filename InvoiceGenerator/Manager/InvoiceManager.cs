using InvoiceGenerator.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceGenerator.Manager
{
    public class InvoiceManager
    {
        public InvoiceConfiguration Configuration { get; set; }

        private string configurationPath;

        public InvoiceManager(string configurationPath)
        {
            this.configurationPath = configurationPath;

            Configuration = null!;
            InitializeConfiguration();
        }

        public InvoiceInstanceConfiguration GetInvoiceInstance(string path)
        {
            if (!File.Exists(path))
                File.WriteAllText(path, new InvoiceInstanceConfiguration().ToJson());

            string? instanceJson = File.ReadAllText(path);

            if (instanceJson == null)
                throw new Exception("Error when loading instance file!");

            InvoiceInstanceConfiguration? instance = InvoiceInstanceConfiguration.FromJson(instanceJson);

            if (instance == null || !(instance.CanBeConverted ?? false))
                throw new Exception("Error when parsing instance file!");

            return instance;
        }

        private void InitializeConfiguration()
        {
            string? configurationJson = null;

            if (File.Exists(configurationPath))
                configurationJson = File.ReadAllText(configurationPath);

            if(configurationJson == null)
            {
                File.WriteAllText(configurationPath, InvoiceConfiguration.Default.ToJson());
                configurationJson = File.ReadAllText(configurationPath);
            }

            if(configurationJson == null)
                throw new Exception("Error when creating and then loading configuration file!");

            InvoiceConfiguration? loadedConfiguration = InvoiceConfiguration.FromJson(configurationJson);

            if(loadedConfiguration == null)
                throw new Exception("Error when parsing configuration file!");

            Configuration = loadedConfiguration;
        }
    }
}
