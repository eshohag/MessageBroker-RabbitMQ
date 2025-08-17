using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublisherConsoleApp.Models
{
    public sealed class RabbitMqOptions
    {
        public string HostName { get; init; } = "localhost";
        public int Port { get; init; } = 5672;
        public string UserName { get; init; } = "guest";
        public string Password { get; init; } = "guest";
        public string VirtualHost { get; init; } = "/";
        public string Exchange { get; init; } = "";          // default exchange
        public string RoutingKey { get; init; } = "my-queue";

        public int PublisherConfirmTimeoutSeconds { get; init; } = 5;
        public int NetworkRecoveryIntervalSeconds { get; init; } = 10;
        public int HeartbeatSeconds { get; init; } = 60;
        public int RequestedConnectionTimeoutSeconds { get; init; } = 10;
        public ushort RequestedChannelMax { get; init; } = 0;
        public uint RequestedFrameMax { get; init; } = 0;
        public bool AutomaticRecoveryEnabled { get; init; } = true;

        public TopologyOptions Topology { get; init; } = new();
        public RetryOptions Retry { get; init; } = new();

        public sealed class TopologyOptions
        {
            public bool DeclareQueue { get; init; } = true;
            public string QueueName { get; init; } = "my-queue";
            public bool Durable { get; init; } = true;
            public bool Exclusive { get; init; } = false;
            public bool AutoDelete { get; init; } = false;
        }

        public sealed class RetryOptions
        {
            public int RetryCount { get; init; } = 5;
            public int BaseDelayMilliseconds { get; init; } = 200;
        }
    }

}
