namespace SimpleBrokeredMessaging.Messaging;

public enum ChatMessageType
{
    Join,
    Chat,
    Leave
}

public sealed record ChatMessage(string SenderName, string Text, ChatMessageType Type, DateTimeOffset Timestamp);
