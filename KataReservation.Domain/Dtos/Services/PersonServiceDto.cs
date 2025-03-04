using KataReservation.Domain.Models;

namespace KataReservation.Domain.Dtos.Services;

public record PersonServiceDto(int Id, string FirstName, string LastName)
{
    internal PersonServiceDto(Person value) :
        this(value.Id, value.FirstName, value.LastName)
    { }
}