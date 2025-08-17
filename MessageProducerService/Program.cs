
using MessageProducerService.Models;
using MessageProducerService.Provider;
using MessageProducerService.Services;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MessageProducerService
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

            #region Register as a hosted service + as a provider for Message Broker DI
            builder.Services.Configure<BrokerSetting>(builder.Configuration.GetSection("RabbitMQ"));
            builder.Services.AddSingleton<IConnectionProvider, ConnectionProvider>();
            builder.Services.AddHostedService(sp => (ConnectionProvider)sp.GetRequiredService<IConnectionProvider>());
            builder.Services.AddSingleton<IProducerService, ProducerService>();
            #endregion

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
