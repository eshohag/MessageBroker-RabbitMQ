
using MassTransit;
using Producer.WebAPI.Models;

namespace Producer.WebAPI
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

            // Configure MassTransit with RabbitMQ
            var rabbitMqSettings = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();
            builder.Services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((context, cfg) =>
                {
                    // 1. Configure RabbitMQ Host from appsettings.json
                    cfg.Host(new Uri($"rabbitmq://{rabbitMqSettings.HostName}:{rabbitMqSettings.Port}/{rabbitMqSettings.VirtualHost}"), h =>
                    {
                        h.Username(rabbitMqSettings.UserName);
                        h.Password(rabbitMqSettings.Password);
                    });
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
            // 3️⃣ Producer endpoint
            app.MapPost("/order", async (OrderCreated order, IBus bus) =>
            {
                var endpoint = await bus.GetSendEndpoint(new Uri("exchange:order-direct-exchange"));

                await endpoint.Send(order, context =>
                {
                    context.SetRoutingKey("direct-order-key");
                    context.Durable = true;
                });

                return Results.Ok(new { Message = "Order sent successfully", order.OrderId });
            });
            app.Run();
        }
    }
}
