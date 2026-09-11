using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace SimpleBrokeredMessaging.Messaging;

public static class JsonMessageSerializer
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static ServiceBusMessage ToServiceBusMessage<T>(T payload)
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(payload, Options);
        return new ServiceBusMessage(body) { ContentType = "application/json" };
    }

    public static T FromServiceBusMessage<T>(ServiceBusReceivedMessage message)
    {
        return JsonSerializer.Deserialize<T>(message.Body, Options)
            ?? throw new InvalidOperationException($"Message body could not be deserialized to {typeof(T).Name}.");
    }
}
