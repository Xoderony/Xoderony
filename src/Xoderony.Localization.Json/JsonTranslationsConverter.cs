using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xoderony.Localization.Json;

internal sealed class JsonTranslationsConverter : JsonConverter<JsonTranslations> {

    public override JsonTranslations Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType != JsonTokenType.StartObject) {
            throw new JsonException("The translations must be an object.");
        }

        var keyToTranslation = new SortedDictionary<string, string>(StringComparer.Ordinal);
        while (reader.Read()) {
            switch (reader.TokenType) {
                case JsonTokenType.EndObject:
                    return new JsonTranslations(keyToTranslation);
                case JsonTokenType.PropertyName:
                    var key = reader.GetString();
                    if (key is null) {
                        throw new JsonException("The localization key cannot be null.");
                    }

                    if (!reader.Read()) {
                        throw new JsonException($"The localization value '{key}' is incomplete.");
                    }

                    if (reader.TokenType != JsonTokenType.String) {
                        throw new JsonException($"The localization value '{key}' must be a string.");
                    }

                    if (!keyToTranslation.TryAdd(key, reader.GetString()!)) {
                        throw new JsonException($"The localization key '{key}' is duplicated.");
                    }

                    break;
                default:
                    throw new JsonException("The translations JSON object is invalid.");
            }
        }

        throw new JsonException("The translations JSON object is incomplete.");
    }

    public override void Write(Utf8JsonWriter writer, JsonTranslations value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        foreach (var (key, text) in value.KeyToTranslation) {
            writer.WriteString(key, text);
        }

        writer.WriteEndObject();
    }
}
