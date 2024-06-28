namespace ImageBox.Core.IOPath;

internal class IOPathSerializer : JsonConverter<IOPath>
{
    public override IOPath Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected string, got " + reader.TokenType);

        var value = reader.GetString() ?? string.Empty;
        return new IOPath(value);
    }

    public override void Write(Utf8JsonWriter writer, IOPath value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
