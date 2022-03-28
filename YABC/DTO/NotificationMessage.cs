using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace YABC.DTO;

public class NotificationMessage
{
    [JsonConverter(typeof(StringEnumConverter))]
    public MessageType MessageType { get; set; }

    public BlockDTO? Block { get; set; }
}
