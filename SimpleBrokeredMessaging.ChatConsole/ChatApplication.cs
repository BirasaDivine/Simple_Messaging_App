using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Azure.Amqp.Framing;
using SimpleBrokeredMessaging.Messaging;
using static System.Net.Mime.MediaTypeNames;

namespace SimpleBrokeredMessaging.ChatConsole
{
    internal class ChatApplication
    {
        // Set the SERVICEBUS_CONNECTION_STRING environment variable before running
        static string ConnectionString = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING")
            ?? throw new InvalidOperationException("Set the SERVICEBUS_CONNECTION_STRING environment variable to your Service Bus connection string.");
        static string TopicName = "chattopic";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter name:");
            var userName = Console.ReadLine();
            //Create an administration client to manage artifacts
            var serviceBusAdministrationClient = new ServiceBusAdministrationClient(ConnectionString);


            //Create a topic if it does not exist
            if (!await serviceBusAdministrationClient.TopicExistsAsync(TopicName))
            {
                await serviceBusAdministrationClient.CreateTopicAsync(TopicName);
            }

            //Create a temporary subscription for the user if it does not exist
            if (!await serviceBusAdministrationClient.SubscriptionExistsAsync(TopicName , userName))
            {
                var options = new CreateSubscriptionOptions(TopicName, userName)
                {
                    AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
                };
                await serviceBusAdministrationClient.CreateSubscriptionAsync(options);
            }
            //create a service bus client
            var serviceBusClient = new ServiceBusClient(ConnectionString);
            //create a service bus sender
            var serviceBusSender = serviceBusClient.CreateSender(TopicName);
            //Create a messsage processor
            var processor = serviceBusClient.CreateProcessor(TopicName , userName);
            //add handler to process messages
            processor.ProcessMessageAsync += MessageHandler;
            //add handler to process any errors
            processor.ProcessErrorAsync += ErrorHandler;
            //start the message processor
            await processor.StartProcessingAsync();
            //send a hello message
            var helloMessage = JsonMessageSerializer.ToServiceBusMessage(
                new ChatMessage(userName!, $"{userName} has entered the room", ChatMessageType.Join, DateTimeOffset.UtcNow));
            await serviceBusSender.SendMessageAsync(helloMessage);
            while (true)
            {
                var text = Console.ReadLine();
                    if (text == "exit")
                {
                    break;
                }
                var message = JsonMessageSerializer.ToServiceBusMessage(
                    new ChatMessage(userName!, text ?? string.Empty, ChatMessageType.Chat, DateTimeOffset.UtcNow));
                await serviceBusSender.SendMessageAsync(message);
            }
            var goodbyeMessage = JsonMessageSerializer.ToServiceBusMessage(
                new ChatMessage(userName!, $"{userName} has left the room", ChatMessageType.Leave, DateTimeOffset.UtcNow));
            await serviceBusSender.SendMessageAsync(goodbyeMessage);
            // Close the message processor
            await processor.StopProcessingAsync();
            // Close the receiver
            await processor.CloseAsync();
            await serviceBusSender.CloseAsync();
        }
        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var chatMessage = JsonMessageSerializer.FromServiceBusMessage<ChatMessage>(args.Message);
            var line = chatMessage.Type == ChatMessageType.Chat
                ? $"{chatMessage.SenderName} > {chatMessage.Text}"
                : chatMessage.Text;
            Console.WriteLine(line);
            // Complete the message
            await args.CompleteMessageAsync(args.Message);
        }
        static async Task ErrorHandler(ProcessErrorEventArgs args)
        {
            throw new NotImplementedException();
        }

    }

}
