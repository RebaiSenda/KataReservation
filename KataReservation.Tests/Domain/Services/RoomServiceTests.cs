using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using NFluent;
using NSubstitute;
using Xunit;
using KataReservation.Domain.Services;
using KataReservation.Domain.Models;
using KataReservation.Domain.Dtos.Repositories;
using Moq;
using KataReservation.Domain.Interfaces.Repositories;

namespace KataReservation.Tests.Domain.Services;

public class RoomServiceTests
{
    private readonly Mock<IRoomRepository> _mockRoomRepository;
    private readonly RoomService _service;

    public RoomServiceTests()
    {
        _mockRoomRepository = new Mock<IRoomRepository>();
        _service = new RoomService(_mockRoomRepository.Object);
    }

    [Fact]
    public async Task GetRoomsAsync_ReturnsAllRooms()
    {
        var roomDtos = new List<RoomRepositoryDto>
            {
                new RoomRepositoryDto(1, "Room 1"),
                new RoomRepositoryDto(2, "Room 2")
            };

        _mockRoomRepository.Setup(r => r.GetRoomsAsync()).ReturnsAsync(roomDtos);

        var result = await _service.GetRoomsAsync();

        var roomsList = result.ToList();
        Assert.Equal(2, roomsList.Count);
        Assert.Equal(1, roomsList[0].Id);
        Assert.Equal("Room 1", roomsList[0].RoomName);
        Assert.Equal(2, roomsList[1].Id);
        Assert.Equal("Room 2", roomsList[1].RoomName);
    }

    [Fact]
    public async Task GetRoomByIdAsync_WithExistingId_ReturnsRoom()
    {
        var roomDto = new RoomRepositoryDto(1, "Room 1");
        _mockRoomRepository.Setup(r => r.GetRoomByIdAsync(1)).ReturnsAsync(roomDto);

        var result = await _service.GetRoomByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Room 1", result.RoomName);
    }

    [Fact]
    public async Task GetRoomByIdAsync_WithNonExistingId_ReturnsNull()
    {
        _mockRoomRepository.Setup(r => r.GetRoomByIdAsync(999)).ReturnsAsync((RoomRepositoryDto)null);

        var result = await _service.GetRoomByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateRoomAsync_CreatesAndReturnsRoom()
    {
        var roomDto = new RoomRepositoryDto(1, "New Room");
        _mockRoomRepository.Setup(r => r.CreateRoomAsync("New Room")).ReturnsAsync(roomDto);

        var result = await _service.CreateRoomAsync("New Room");

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("New Room", result.RoomName);
    }

    [Fact]
    public async Task UpdateRoomAsync_WithExistingId_UpdatesAndReturnsRoom()
    {
        var roomDto = new RoomRepositoryDto(1, "Updated Room");
        _mockRoomRepository.Setup(r => r.UpdateRoomAsync(1, "Updated Room")).ReturnsAsync(roomDto);

        var result = await _service.UpdateRoomAsync(1, "Updated Room");

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Room", result.RoomName);
    }

    [Fact]
    public async Task UpdateRoomAsync_WithNonExistingId_ReturnsNull()
    {
        _mockRoomRepository.Setup(r => r.UpdateRoomAsync(999, "Updated Room")).ReturnsAsync((RoomRepositoryDto)null);

        var result = await _service.UpdateRoomAsync(999, "Updated Room");

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteRoomAsync_WithExistingId_ReturnsTrue()
    {
        _mockRoomRepository.Setup(r => r.DeleteRoomAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteRoomAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteRoomAsync_WithNonExistingId_ReturnsFalse()
    {
        _mockRoomRepository.Setup(r => r.DeleteRoomAsync(999)).ReturnsAsync(false);

        var result = await _service.DeleteRoomAsync(999);

        Assert.False(result);
    }
}

