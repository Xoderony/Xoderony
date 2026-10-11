using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xoderony.Localization.Json;

internal sealed class JsonKeyGroupConverter : JsonConverter<JsonKeyGroup> {

    public override JsonKeyGroup Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType != JsonTokenType.StartObject) {
            throw new JsonException("The keys value must be an object.");
        }

        var group = new JsonKeyGroup(string.Empty, string.Empty);
        Populate(ref reader, group);
        return group;

        static void Populate(ref Utf8JsonReader reader, JsonKeyGroup group) {
            while (reader.Read()) {
                switch (reader.TokenType) {
                    case JsonTokenType.EndObject:
                        return;
                    case JsonTokenType.PropertyName:
                        var localKey = reader.GetString();
                        if (!JsonKeyNode.IsValidLocalKey(localKey)) {
                            throw new JsonException($"The local key '{localKey}' cannot be empty or contain '.'.");
                        }

                        if (!reader.Read()) {
                            throw new JsonException($"The keys value '{JsonKeyNode.CombineKey(group.FullKey, localKey)}' is incomplete.");
                        }

                        switch (reader.TokenType) {
                            case JsonTokenType.Null:
                                group.AddEntry(localKey);
                                break;
                            case JsonTokenType.StartObject:
                                Populate(ref reader, group.AddGroup(localKey));
                                break;
                            default:
                                throw new JsonException($"The keys value '{JsonKeyNode.CombineKey(group.FullKey, localKey)}' must be null or an object.");
                        }

                        break;
                    default:
                        throw new JsonException("The keys JSON object is invalid.");
                }
            }

            throw new JsonException("The keys JSON object is incomplete.");
        }
    }

    public override void Write(Utf8JsonWriter writer, JsonKeyGroup value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        foreach (var (localKey, child) in value.LocalKeyToChild) {
            writer.WritePropertyName(localKey);
            if (child is JsonKeyGroup childGroup) {
                Write(writer, childGroup, options);
                continue;
            }

            writer.WriteNullValue();
        }
        writer.WriteEndObject();
    }
}
