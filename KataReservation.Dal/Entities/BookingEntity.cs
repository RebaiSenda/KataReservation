using System;
using System.Collections.Generic;

namespace KataReservation.Dal.Entities;

public partial class BookingEntity
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public int PersonId { get; set; }

    public DateTime BookingDate { get; set; }

    public int StartSlot { get; set; }

    public int EndSlot { get; set; }

    public virtual PersonEntity Person { get; set; } = null!;

    public virtual RoomEntity Room { get; set; } = null!;
}
