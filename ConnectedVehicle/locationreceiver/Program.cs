﻿using System;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace locationreceiver
{
    class Program
    {
        #region secrets
        //All secrets are configured with Secret Manager Tool! 
        //You can see each secret setup above the secret it self!

        //dotnet user-secrets set event-hub-connectionstring "{Event Hubs connection string}"
        private static string EventHubConnectionStringSecret = "event-hub-connectionstring";

        //dotnet user-secrets set storage-account-key "{Storage account key}"
        private static string StorageAccountConnectionStringSecret = "storage-account-key";
        #endregion

        private static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
            InitializeConfiguration();
            MainAsync(args).GetAwaiter().GetResult();
        }

        //Load secrets and appsettings.json
        private static void InitializeConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddUserSecrets<Program>();

            Configuration = builder.Build();
        }

        private static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Registering EventProcessor...");

            //Event hub settings
            string EventHubName = Configuration["EventHubName"];
            string EventHubConnectionString = Configuration[EventHubConnectionStringSecret];

            //Storage settings
            string StorageContainerName = Configuration["StorageContainerName"];
            string StorageConnectionString = Configuration[StorageAccountConnectionStringSecret];

            //Setup event processor host
            var eventProcessorHost = new EventProcessorHost(
                EventHubName,
                PartitionReceiver.DefaultConsumerGroupName,
                EventHubConnectionString,
                StorageConnectionString,
                StorageContainerName);

            // Registers the Event Processor Host and starts receiving messages
            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>();

            Console.WriteLine("Receiving. Press ENTER to stop worker.");
            Console.ReadLine();

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }
    }
}
