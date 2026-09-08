# Simple Messaging App

An Azure Service Bus demo built as a .NET solution with four projects:

- **SimpleBrokeredMessaging** — ASP.NET Core Web API host.
- **SimpleBrokeredMessaging.Sender** — console app that sends messages to a queue.
- **SimpleBrokeredMessaging.Receiver** — console app that receives messages from a queue.
- **SimpleBrokeredMessaging.ChatConsole** — console app implementing a topic-based chat room.

## Configuration

The console apps read the Service Bus connection string from an environment
variable instead of hardcoding it:

```
setx SERVICEBUS_CONNECTION_STRING "Endpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=...;SharedAccessKey=..."
```

Set this before running `SimpleBrokeredMessaging.Sender`, `SimpleBrokeredMessaging.Receiver`,
or `SimpleBrokeredMessaging.ChatConsole`.
