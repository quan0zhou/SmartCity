using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCityWebApi.ViewModels
{
    public record CustSpaceViewModel
    {
        public long? SpaceId { get; set; }
        public string SpaceName { get; set; } = default!;

        public string SpaceAddress { get; set; } = default!;

        public string ContactName { get; set; } = default!;

        public string ContactPhone { get; set; } = default!;

        public string Remark { get; set; } = default!;

        public int SpaceType { get; set; }

    }
}
