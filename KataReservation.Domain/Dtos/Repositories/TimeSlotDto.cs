using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KataReservation.Domain.Dtos.Repositories
{
    public record TimeSlotDto(int Hour, bool IsAvailable);
}
