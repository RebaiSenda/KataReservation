using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KataReservation.Domain.Dtos.Services
{
    public class AvailableSlotDto
    {
        public int StartSlot { get; }
        public int EndSlot { get; }

        public AvailableSlotDto(int startSlot, int endSlot)
        {
            StartSlot = startSlot;
            EndSlot = endSlot;
        }
    }
}
