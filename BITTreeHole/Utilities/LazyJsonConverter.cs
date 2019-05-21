using System;
using Newtonsoft.Json;

namespace BITTreeHole.Utilities
{
    /// <summary>
    /// 为 <see cref="Lazy{T}"/> 提供 <see cref="JsonConverter{Lazy{T}}"/> 实现。
    /// </summary>
    /// <typeparam name="T"><see cref="Lazy{T}"/> 的被包装类型。</typeparam>
    public sealed class LazyJsonConverter<T> : JsonConverter<Lazy<T>>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, Lazy<T> value, JsonSerializer serializer)
        {
            if (value.IsValueCreated)
            {
                writer.WriteValue(value.Value);
            }
            else
            {
                writer.WriteUndefined();
            }
        }

        /// <inheritdoc />
        public override Lazy<T> ReadJson(JsonReader reader, Type objectType, Lazy<T> existingValue, 
                                         bool hasExistingValue, JsonSerializer serializer)
        {
            var value = serializer.Deserialize<T>(reader);
            return new Lazy<T>(value);
        }
    }
}
