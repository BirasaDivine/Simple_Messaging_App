using Azure.Messaging.ServiceBus;
using System;

namespace SimpleBrokeredMessaging.Sender
{
    internal class SenderConsole
    {
        // Set the SERVICEBUS_CONNECTION_STRING environment variable before running
        static string ConnectionString = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING")
            ?? throw new InvalidOperationException("Set the SERVICEBUS_CONNECTION_STRING environment variable to your Service Bus connection string.");
        static string QueueName = "demoqueue";
        static string Sentence = "Microsoft zure Service Bus.";

        static async Task Main(string[] args)
        {
            //create a service bus client
            var client = new ServiceBusClient(ConnectionString);


            // create a service bus sender

            var sender = client.CreateSender(QueueName);

            //send some message
            Console.WriteLine("Sending messages");
            foreach (var character in Sentence)
            {
                var message = new ServiceBusMessage(character.ToString());
                await sender.SendMessageAsync(message);
                Console.WriteLine($"Sent : {character}");
            }
            //close the sender
            await sender.CloseAsync();
            Console.WriteLine("Sent messages.");
            Console.ReadLine();
        }

        }
    }
