using KataReservation.Domain.Dtos.Services;

namespace KataReservation.Tests.Api.Dtos
{
    public record PersonRepositoryDtoWithCase(int Id, string FirstName, string LastName)
    {
        private readonly string _firstName = FirstName;
        private readonly string _lastName = LastName;

        public string FirstName => CapitalizeFirstLetter(_firstName);
        public string LastName => CapitalizeFirstLetter(_lastName);

        public PersonRepositoryDtoWithCase(PersonServiceDto value) :
            this(value.Id, value.FirstName, value.LastName)
        { }

        private static string CapitalizeFirstLetter(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            if (text.Length == 1)
                return text.ToUpper();

            return char.ToUpper(text[0]) + text.Substring(1);
        }
    }
}
