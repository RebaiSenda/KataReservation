using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KataReservation.MessagingService.Configuration;
using KataReservation.MessagingService.Interfaces;
using KataReservation.MessagingService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KataReservation.MessagingService.DependencyInjection
{
    public static class MessagingServiceExtensions
    {
        public static IServiceCollection AddMessagingServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuration RabbitMQ
            services.Configure<RabbitMQConfig>(configuration.GetSection("RabbitMQ"));

            // Enregistrer le service de messagerie en séparant l'implémentation de l'interface
            // RabbitMQService est uniquement enregistré comme IMessagePublisher personnalisé
            services.AddSingleton<IMessagePublisher, RabbitMQService>();

            return services;
        }
    }
}