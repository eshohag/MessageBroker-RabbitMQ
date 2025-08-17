using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublisherConsoleApp
{
    public interface IRabbitMqPublisher : IAsyncDisposable, IDisposable
    {
        Task PublishAsync(
            string exchange,
            string routingKey,
            ReadOnlyMemory<byte> body,
            IBasicProperties? properties = null,
            bool mandatory = true,
            CancellationToken ct = default);

        IBasicProperties CreateBasicProperties();
    }
}
