using Azure.Messaging.ServiceBus;
using SimpleBrokeredMessaging.Messaging;
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

            var messages = Sentence
            .Split(' ')
            .Select((word, index) => JsonMessageSerializer.ToServiceBusMessage(new DemoMessage(index, word)))
            .ToList();

            Console.WriteLine("Sending messages");
            await SendInBatchesAsync(sender, messages);
            Console.WriteLine("Sent messages.");

            //close the sender
            await sender.CloseAsync();
            Console.ReadLine();
        }

        static async Task SendInBatchesAsync(ServiceBusSender sender, IReadOnlyList<ServiceBusMessage> messages)
        {
            var batch = await sender.CreateMessageBatchAsync();
            var batchesSent = 0;

            foreach (var message in messages)
            {
                if (batch.TryAddMessage(message))
                {
                    continue;
                }

                if (batch.Count == 0)
                {
                    throw new InvalidOperationException($"Message is too large to fit in an empty batch: \"{message.Body}\"");
                }

                await sender.SendMessagesAsync(batch);
                Console.WriteLine($"Sent batch of {batch.Count} message(s).");
                batchesSent++;
                batch.Dispose();

                batch = await sender.CreateMessageBatchAsync();
                if (!batch.TryAddMessage(message))
                {
                    throw new InvalidOperationException($"Message is too large to fit in an empty batch: \"{message.Body}\"");
                }
            }

            if (batch.Count > 0)
            {
                await sender.SendMessagesAsync(batch);
                Console.WriteLine($"Sent batch of {batch.Count} message(s).");
                batchesSent++;
            }

            batch.Dispose();
            Console.WriteLine($"Sent {messages.Count} message(s) in {batchesSent} batch(es).");
        }
    }
}
