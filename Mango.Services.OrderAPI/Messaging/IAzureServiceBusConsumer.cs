namespace Mango.Services.OrderAPI.Messaging
{
    public interface IAzureServiceBusConsumer
    {
        Task StartAsync();
        Task StopAsync();
    }
}
