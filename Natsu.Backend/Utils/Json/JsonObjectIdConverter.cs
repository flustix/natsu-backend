using MongoDB.Bson;
using Newtonsoft.Json;

namespace Natsu.Backend.Utils.Json;

public class JsonObjectIdConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is ObjectId objectId)
            writer.WriteValue(objectId.ToString());
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.Value is string value)
            return ObjectId.Parse(value);

        return null;
    }

    public override bool CanConvert(Type objectType) => objectType == typeof(ObjectId);
}
