using NFluent;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using KataReservation.Domain.Models;
using KataReservation.Dal.Entities;
using KataReservation.Domain.Dtos.Repositories;
using Room = KataReservation.Domain.Models.Room;

namespace KataReservation.Tests.Domain.Models;

public class RoomTests
{
        [Fact]
        public void Room_Constructor_SetsProperties()
        {
            var room = new Room(1, "Test Room");

            Assert.Equal(1, room.Id);
            Assert.Equal("Test Room", room.RoomName);
        }

        [Fact]
        public void Room_UpdateName_ChangesRoomName()
        {
            var room = new Room(1, "Old Name");

            room.UpdateName("New Name");

            Assert.Equal("New Name", room.RoomName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Room_UpdateName_WithInvalidName_ThrowsArgumentException(string invalidName)
        {
            var room = new Room(1, "Valid Name");

            var exception = Assert.Throws<ArgumentException>(() => room.UpdateName(invalidName));
            Assert.Contains("Room name cannot be empty or whitespace", exception.Message);
        }

        [Fact]
        public void Room_ToString_ReturnsFormattedString()
        {
            var room = new Room(1, "Test Room");
            var expected = "Room 1: Test Room";

            var result = room.ToString();

            Assert.Equal(expected, result);
        }
    }
