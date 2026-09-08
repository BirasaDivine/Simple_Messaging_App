using Azure.Messaging.ServiceBus;

namespace SimpleBrokeredMessaging.Receiver
{
    internal class ReceiverConsole {
        // Set the SERVICEBUS_CONNECTION_STRING environment variable before running
        static string ConnectionString = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING")
            ?? throw new InvalidOperationException("Set the SERVICEBUS_CONNECTION_STRING environment variable to your Service Bus connection string.");
        static string QueueName = "demoqueue";
        static async Task Main(string[] args)
        {
            //Create a service bus client
            var client = new ServiceBusClient(ConnectionString);


            //Create a service bus receiver
            var receiver = client.CreateReceiver(QueueName);


            //Receive the messages
            Console.WriteLine("Receive messages....");
            while (true)
            {
                var message = await receiver.ReceiveMessageAsync();
                if (message != null)
                {
                    Console.Write(message.Body.ToString());

                    //complete the message
                    await receiver.CompleteMessageAsync(message);
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("All messages received");
                    break;
                }
            }

            // Close the receiver
            await receiver.CloseAsync();
        }

    }

}
