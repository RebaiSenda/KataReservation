﻿using System.ComponentModel.DataAnnotations;

namespace KataReservation.Api.Dtos.Requests
{
    public record CreateRoomRequest(
        [Required]
    [StringLength(50, MinimumLength = 2)]
    string RoomName
    );
}
