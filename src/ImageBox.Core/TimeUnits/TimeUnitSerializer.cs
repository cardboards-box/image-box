namespace ImageBox.Core.TimeUnits;

internal class TimeUnitSerializer : JsonConverter<TimeUnit>
{
    public override TimeUnit Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value)) return TimeUnit.Zero;
            return TimeUnit.Parse(value);
        }

        var node = JsonNode.Parse(ref reader);
        if (node is null) return TimeUnit.Zero;

        var unit = (double?)node["value"]?.AsValue();
        if (unit is null) return TimeUnit.Zero;

        var type = node["type"]?.ToString();
        if (type is null) return TimeUnit.Zero;

        if (!Enum.TryParse<TimeUnitType>(type, true, out var timeUnit))
            timeUnit = TimeUnitType.Millisecond;

        return new TimeUnit(timeUnit, unit.Value);
    }

    public override void Write(Utf8JsonWriter writer, TimeUnit value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
