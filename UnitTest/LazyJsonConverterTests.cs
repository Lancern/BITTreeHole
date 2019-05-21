using System;
using BITTreeHole.Utilities;
using Newtonsoft.Json;
using Xunit;

namespace BITTreeHole.Test.UnitTest
{
    public class LazyJsonConverterTests
    {
        private sealed class MockObject
        {
            [JsonConstructor]
            public MockObject()
            {
                LazyValue = new Lazy<int>();
            }
            
            [JsonProperty("lazyValue")]
            [JsonConverter(typeof(LazyJsonConverter<int>))]
            public Lazy<int> LazyValue { get; set; }
        }
        
        [Fact]
        public void UndefinedValue()
        {
            var json = "{ \"lazyValue\": undefined }";
            var mock = JsonConvert.DeserializeObject<MockObject>(json);
            
            Assert.False(mock.LazyValue.IsValueCreated);
        }

        [Fact]
        public void DefinedValue()
        {
            var json = "{ \"lazyValue\": 325 }";
            var mock = JsonConvert.DeserializeObject<MockObject>(json);
            
            Assert.True(mock.LazyValue.IsValueCreated);
            Assert.Equal(325, mock.LazyValue.Value);
        }
    }
}
