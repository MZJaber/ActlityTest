namespace ActilityService.Cash;

using ActilityService.Modules;
using System.Collections.Concurrent;

public static class MessageQueueCash
{
    public static ConcurrentQueue<PEPayloadMessage> MessageQueue { get; } = new ConcurrentQueue<PEPayloadMessage>();
}
