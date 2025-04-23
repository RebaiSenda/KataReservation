using KataReservation.Domain.Models;

namespace KataReservation.Api.Dtos.Responses
{
    public record SimpleDataItemResponse(int Id, string Name)
    {
        public SimpleDataItemResponse(SimpleDataItem dto) : this(dto.Id, dto.Name) { }
    }

    public record SimpleDataResponse(string Message, IEnumerable<SimpleDataItemResponse> Items)
    {
        public SimpleDataResponse(Domain.Models.SimpleDataResponse domainResponse)
            : this(domainResponse.Message, domainResponse.Items.Select(item => new SimpleDataItemResponse(item))) { }
    }
}