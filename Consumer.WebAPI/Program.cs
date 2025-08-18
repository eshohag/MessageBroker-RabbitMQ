
using Consumer.WebAPI.Models;
using Consumer.WebAPI.Services;
using MassTransit;
using RabbitMQ.Client;

namespace Consumer.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var rabbitMqSettings = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();
            builder.Services.AddMassTransit(config =>
            {
                // 1. Register consumer
                config.AddConsumer<OrderCreatedConsumer>();
                config.UsingRabbitMq((context, cfg) =>
                {
                    // 1. Configure RabbitMQ Host from appsettings.json
                    cfg.Host(new Uri($"rabbitmq://{rabbitMqSettings.HostName}:{rabbitMqSettings.Port}/{rabbitMqSettings.VirtualHost}"), h =>
                    {
                        h.Username(rabbitMqSettings.UserName);
                        h.Password(rabbitMqSettings.Password);
                    });

                    cfg.ReceiveEndpoint("my.orders.mail-queue", e =>
                    {
                        e.ConfigureConsumeTopology = false; // Stops auto-binding to message type exchange

                        e.Bind("my.orders.exchange", s => {
                            s.RoutingKey = "orders.*";
                            s.ExchangeType = "topic";
                        });

                        e.ConfigureConsumer<OrderCreatedConsumer>(context);
                    });

                    // Configure queue and direct exchange
                    //cfg.ReceiveEndpoint("order-queue", e =>
                    //{
                    //    e.ConfigureConsumeTopology = false;

                    //    e.Bind("order-direct-exchange", bind =>
                    //    {
                    //        bind.ExchangeType = ExchangeType.Direct;
                    //        bind.RoutingKey = "direct-order-key";
                    //    });

                    //    e.ConfigureConsumer<OrderCreatedConsumer>(context);
                    //    e.PrefetchCount = (ushort)rabbitMqSettings.PrefetchCount;
                    //});

                    //// 2. Configure Publish Middleware
                    //cfg.ConfigurePublish(y =>
                    //{
                    //    // Retry policy for message publishing, If publish fails (network issue, broker unavailable), it retries 3 times with 1 second interval between retries.
                    //    y.UseRetry(r => r.Interval(3, TimeSpan.FromSeconds(1)));

                    //    /* breaker for publishing pipeline repeated failures.
                    //        Tracks failures over a 1-minute window.
                    //        If more than 15 failures occur in that period, the circuit “trips” → stops trying immediately.
                    //        After the cooldown, it’ll try again automatically.     
                    //     */
                    //    y.UseCircuitBreaker(cb =>
                    //    {
                    //        cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    //        cb.TripThreshold = 15;
                    //    });
                    //});
                });
            });
            builder.Services.AddMassTransitHostedService();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
