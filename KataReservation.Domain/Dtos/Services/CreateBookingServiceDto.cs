using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KataReservation.Domain.Dtos.Services
{
    public record CreateBookingServiceDto(
          int RoomId,
          int PersonId,
          DateTime BookingDate,
          int StartSlot,
          int EndSlot
      );
}
