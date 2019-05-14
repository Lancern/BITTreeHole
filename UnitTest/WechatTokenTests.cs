using System;
using BITTreeHole.Services;
using Newtonsoft.Json.Linq;
using Xunit;

namespace BITTreeHole.Test.UnitTest
{
    public class WechatTokenTests
    {
        [Fact]
        public void ReadFromWechatJsonNull()
        {
            Assert.Throws<ArgumentNullException>(() => WechatToken.FromWechatJson((string) null));
            Assert.Throws<ArgumentNullException>(() => WechatToken.FromWechatJson((JObject) null));
        }
        
        [Fact]
        public void ReadFromValidWechatJsonString()
        {
            var json = "{" +
                       "\"access_token\": \"ACCESS_TOKEN\"," +
                       "\"expires_in\": 7200, " +
                       "\"refresh_token\": \"REFRESH_TOKEN\", " +
                       "\"openid\": \"OPENID\", " +
                       "\"scope\": \"SCOPE1,SCOPE2\"," +
                       "\"unionid\": \"o6_bmasdasdsad6_2sgVt7hMZOPfL\"" +
                       "}";
            var token = WechatToken.FromWechatJson(json);
            
            Assert.Equal("ACCESS_TOKEN", token.AccessToken);
            Assert.True(Math.Abs((token.ExpireTime - DateTime.Now).TotalSeconds - 7200) < 10);
            Assert.Equal("REFRESH_TOKEN", token.RefreshToken);
            Assert.Equal("OPENID", token.OpenId);
            Assert.Equal(new string[] { "SCOPE1", "SCOPE2" }, token.Scopes);
            Assert.Equal("o6_bmasdasdsad6_2sgVt7hMZOPfL", token.UnionId);
        }

        [Fact]
        public void ReadFromValidWechatJsonStringInvalidNoUnionId()
        {
            var json = "{" +
                       "\"access_token\": \"ACCESS_TOKEN\"," +
                       "\"expires_in\": 7200, " +
                       "\"refresh_token\": \"REFRESH_TOKEN\", " +
                       "\"openid\": \"OPENID\", " +
                       "\"scope\": \"SCOPE1,SCOPE2\"," +
                       "}";
            var token = WechatToken.FromWechatJson(json);
            
            Assert.Equal("ACCESS_TOKEN", token.AccessToken);
            Assert.True(Math.Abs((token.ExpireTime - DateTime.Now).TotalSeconds - 7200) < 10);
            Assert.Equal("REFRESH_TOKEN", token.RefreshToken);
            Assert.Equal("OPENID", token.OpenId);
            Assert.Equal(new string[] { "SCOPE1", "SCOPE2" }, token.Scopes);
            Assert.Null(token.UnionId);
        }

        [Fact]
        public void ReadFromInvalidWechatJsonString()
        {
            var json = "{ \"errcode\": 40029, \"errmsg\": \"invalid code\" }";
            Assert.ThrowsAny<Exception>(() => WechatToken.FromWechatJson(json));
        }
    }
}
