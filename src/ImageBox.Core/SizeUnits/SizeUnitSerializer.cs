namespace ImageBox.Core.SizeUnits;

internal class SizeUnitSerializer : JsonConverter<SizeUnit>
{
    public override SizeUnit Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value)) return SizeUnit.Zero;
            return SizeUnit.Parse(value);
        }

        var node = JsonNode.Parse(ref reader);
        if (node is null) return SizeUnit.Zero;

        var unit = (double?)node["value"]?.AsValue();
        if (unit is null) return SizeUnit.Zero;

        var type = node["type"]?.ToString();
        if (type is null) return SizeUnit.Zero;

        if (!Enum.TryParse<SizeUnitType>(type, true, out var sizeUnit))
            sizeUnit = SizeUnitType.Pixel;

        return new SizeUnit(sizeUnit, unit.Value);
    }

    public override void Write(Utf8JsonWriter writer, SizeUnit value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
