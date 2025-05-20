using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KataReservation.MessagingService.Models;

namespace KataReservation.MessagingService.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishBookingCreatedAsync(BookingNotificationMessage message);
        Task PublishBookingDeletedAsync(BookingNotificationMessage message);
        Task PublishBookingUpdatedAsync(BookingNotificationMessage message);
    }
}
