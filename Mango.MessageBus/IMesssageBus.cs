namespace Mango.MessageBus
{
    public interface IMesssageBus
    {
        Task PublishMessage(BaseMessage message, string topicName);
    }
}