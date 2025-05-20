using Microsoft.Extensions.Logging;
using KataReservation.Domain.Interfaces.Services;
using KataReservation.Api.Controllers;
using KataReservation.Api.Dtos.Requests;
using KataReservation.Api.Dtos.Responses;
using KataReservation.Domain.Dtos.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using KataReservation.MessagingService.Interfaces;
using KataReservation.MessagingService.Models;
using KataReservation.MessagingService.Configuration;
using KataReservation.MessagingService.Services;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace KataReservation.Tests.Api.Controllers;

public class RabbitMQServiceTests : IDisposable
{
    private readonly Mock<IOptions<RabbitMQConfig>> _mockOptions;
    private readonly Mock<IConnection> _mockConnection;
    private readonly Mock<IModel> _mockChannel;
    private readonly Mock<ConnectionFactory> _mockConnectionFactory;
    private readonly RabbitMQConfig _config;
    private RabbitMQService _service;

    public RabbitMQServiceTests()
    {
        // Configuration pour les tests
        _config = new RabbitMQConfig
        {
            HostName = "localhost",
            Username = "guest",
            Password = "guest",
            BookingExchange = "booking_exchange",
            BookingCreatedQueue = "booking_created_queue",
            BookingCreatedRoutingKey = "booking.created",
            BookingUpdatedQueue = "booking_updated_queue",
            BookingUpdatedRoutingKey = "booking.updated",
            BookingDeletedQueue = "booking_deleted_queue",
            BookingDeletedRoutingKey = "booking.deleted"
        };

        _mockOptions = new Mock<IOptions<RabbitMQConfig>>();
        _mockOptions.Setup(o => o.Value).Returns(_config);

        _mockChannel = new Mock<IModel>();
        _mockConnection = new Mock<IConnection>();
        _mockConnection.Setup(c => c.CreateModel()).Returns(_mockChannel.Object);

        _mockConnectionFactory = new Mock<ConnectionFactory>();
        _mockConnectionFactory.Setup(f => f.CreateConnection()).Returns(_mockConnection.Object);
    }

    private RabbitMQService CreateServiceWithMocks()
    {
        // Utilisation de la réflexion pour créer le service avec les mocks
        // Cela nous permet de contourner la création directe de ConnectionFactory dans le constructeur
        var service = new RabbitMQServiceTestable(_mockOptions.Object, _mockConnection.Object, _mockChannel.Object);
        return service;
    }

    [Fact]
    public void Constructor_ShouldDeclareExchangeAndQueues()
    {
        // Arrange & Act
        _service = CreateServiceWithMocks();

        // Assert - Vérifier que l'échange est déclaré
        _mockChannel.Verify(c => c.ExchangeDeclare(
            _config.BookingExchange,
            "direct",
            true,
            false,
            It.IsAny<IDictionary<string, object>>()),
            Times.Once);

        // Vérifier que les files d'attente sont déclarées
        _mockChannel.Verify(c => c.QueueDeclare(
            _config.BookingCreatedQueue,
            true,
            false,
            false,
            It.IsAny<IDictionary<string, object>>()),
            Times.Once);

        _mockChannel.Verify(c => c.QueueDeclare(
            _config.BookingDeletedQueue,
            true,
            false,
            false,
            It.IsAny<IDictionary<string, object>>()),
            Times.Once);

        _mockChannel.Verify(c => c.QueueDeclare(
            _config.BookingUpdatedQueue,
            true,
            false,
            false,
            It.IsAny<IDictionary<string, object>>()),
            Times.Once);

        // Vérifier les bindings
        _mockChannel.Verify(c => c.QueueBind(
            _config.BookingCreatedQueue,
            _config.BookingExchange,
            _config.BookingCreatedRoutingKey,
            It.IsAny<IDictionary<string, object>>()),
            Times.Once);

        _mockChannel.Verify(c => c.QueueBind(
            _config.BookingDeletedQueue,
            _config.BookingExchange,
            _config.BookingDeletedRoutingKey,
            It.IsAny<IDictionary<string, object>>()),
            Times.Once);

        _mockChannel.Verify(c => c.QueueBind(
            _config.BookingUpdatedQueue,
            _config.BookingExchange,
            _config.BookingUpdatedRoutingKey,
            It.IsAny<IDictionary<string, object>>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishBookingCreatedAsync_ShouldPublishToCorrectExchangeAndRoutingKey()
    {
        // Arrange
        _service = CreateServiceWithMocks();
        var bookingNotification = new BookingNotificationMessage
        {
            BookingId = 1001,
            RoomId = 2001,
            PersonId = 3001,
            BookingDate = DateTime.Now,
            StartSlot = 10,
            EndSlot = 15,
            Status = "Created"
        };

        // Setup mock for verification
        _mockChannel.Setup(c => c.CreateBasicProperties()).Returns(Mock.Of<IBasicProperties>());

        // Act
        await _service.PublishBookingCreatedAsync(bookingNotification);

        // Assert
        VerifyBasicPublish(_config.BookingExchange, _config.BookingCreatedRoutingKey, bookingNotification);
    }

    [Fact]
    public async Task PublishBookingUpdatedAsync_ShouldPublishToCorrectExchangeAndRoutingKey()
    {
        // Arrange
        _service = CreateServiceWithMocks();
        var bookingNotification = new BookingNotificationMessage
        {
            BookingId = 1002,
            RoomId = 2002,
            PersonId = 3002,
            BookingDate = DateTime.Now,
            StartSlot = 12,
            EndSlot = 18,
            Status = "Updated"
        };

        // Setup mock for verification
        _mockChannel.Setup(c => c.CreateBasicProperties()).Returns(Mock.Of<IBasicProperties>());

        // Act
        await _service.PublishBookingUpdatedAsync(bookingNotification);

        // Assert
        VerifyBasicPublish(_config.BookingExchange, _config.BookingUpdatedRoutingKey, bookingNotification);
    }

    [Fact]
    public async Task PublishBookingDeletedAsync_ShouldPublishToCorrectExchangeAndRoutingKey()
    {
        // Arrange
        _service = CreateServiceWithMocks();
        var bookingNotification = new BookingNotificationMessage
        {
            BookingId = 1003,
            RoomId = 2003,
            PersonId = 3003,
            BookingDate = DateTime.Now,
            StartSlot = 14,
            EndSlot = 16,
            Status = "Deleted"
        };

        // Setup mock for verification
        _mockChannel.Setup(c => c.CreateBasicProperties()).Returns(Mock.Of<IBasicProperties>());

        // Act
        await _service.PublishBookingDeletedAsync(bookingNotification);

        // Assert
        VerifyBasicPublish(_config.BookingExchange, _config.BookingDeletedRoutingKey, bookingNotification);
    }

    [Fact]
    public void Dispose_ShouldDisposeChannelAndConnection()
    {
        // Arrange
        _service = CreateServiceWithMocks();

        // Act
        _service.Dispose();

        // Assert
        _mockChannel.Verify(c => c.Dispose(), Times.Once);
        _mockConnection.Verify(c => c.Dispose(), Times.Once);
    }

    private void VerifyBasicPublish<T>(string exchange, string routingKey, T expectedMessage)
    {
        // Vérifier que BasicPublish a été appelé avec les bons paramètres
        _mockChannel.Verify(c => c.BasicPublish(
            It.Is<string>(e => e == exchange),
            It.Is<string>(r => r == routingKey),
            It.IsAny<bool>(),
            It.IsAny<IBasicProperties>(),
            It.Is<ReadOnlyMemory<byte>>(b => MatchesMessage(b, expectedMessage))),
            Times.Once);
    }

    private bool MatchesMessage<T>(ReadOnlyMemory<byte> actualBody, T expectedMessage)
    {
        // Conversion du message attendu en JSON puis en bytes pour comparaison
        var expectedJson = JsonSerializer.Serialize(expectedMessage);
        var expectedBytes = Encoding.UTF8.GetBytes(expectedJson);

        // Convertir ReadOnlyMemory<byte> en tableau d'octets pour la comparaison
        var actualBytes = actualBody.ToArray();

        // Retourner true si les contenus correspondent
        return Encoding.UTF8.GetString(actualBytes) == expectedJson;
    }

    public void Dispose()
    {
        _service?.Dispose();
    }
}

// Classe testable qui nous permet d'injecter des mocks
public class RabbitMQServiceTestable : RabbitMQService
{
    public RabbitMQServiceTestable(IOptions<RabbitMQConfig> options, IConnection connection, IModel channel)
        : base(options)
    {
        // Cette classe nous permet de contourner la création de ConnectionFactory dans le constructeur parent
        // en remplaçant les objets internes avec nos mocks
        SetConnection(connection);
        SetChannel(channel);
    }

    // Méthodes pour accéder aux champs privés du parent via réflexion
    private void SetConnection(IConnection connection)
    {
        var field = typeof(RabbitMQService).GetField("_connection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(this, connection);
    }

    private void SetChannel(IModel channel)
    {
        var field = typeof(RabbitMQService).GetField("_channel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(this, channel);
    }
}
