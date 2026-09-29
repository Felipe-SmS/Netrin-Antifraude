namespace Netrin.Antifraude.Application.RabbitMQ
{
    using MassTransit;

    public class Publisher
    {
        private readonly IPublishEndpoint _endpoint;

        public Publisher(IPublishEndpoint endpoint)
        {
            _endpoint = endpoint;
        }

        public Task PublishAsync<T>(T message, CancellationToken cancelamento = default) where T : class
        {
            return _endpoint.Publish(message, cancelamento);
        }
    }
}
