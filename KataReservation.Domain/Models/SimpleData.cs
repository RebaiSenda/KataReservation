using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KataReservation.Domain.Models
{
    public class SimpleDataItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public SimpleDataItem(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class SimpleDataResponse
    {
        public string Message { get; set; }
        public IEnumerable<SimpleDataItem> Items { get; set; }

        public SimpleDataResponse(string message, IEnumerable<SimpleDataItem> items)
        {
            Message = message;
            Items = items;
        }
    }
}
