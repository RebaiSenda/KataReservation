using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("KataReservation.Tests")]

namespace KataReservation.Domain.Models;

public record Person(int Id, string FirstName, string LastName);