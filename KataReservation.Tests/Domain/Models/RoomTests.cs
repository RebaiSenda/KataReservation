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

        Check.That(room.Id).IsEqualTo(1);
        Check.That(room.RoomName).IsEqualTo("Test Room");
    }
}