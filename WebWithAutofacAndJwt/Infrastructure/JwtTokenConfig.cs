using System.Text.Json.Serialization;

namespace WebWithAutofacAndJwt.Infrastructure
{
    /// <summary>
    /// jwt选项
    /// </summary>
    public class JwtTokenConfig
    {
        /// <summary>
        /// 自定义密钥
        /// </summary>
        [JsonPropertyName("secret")]
        public string Secret { get; set; }

        /// <summary>
        /// 发行人
        /// </summary>
        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        /// <summary>
        /// 接受者
        /// </summary>
        [JsonPropertyName("audience")]
        public string Audience { get; set; }

        /// <summary>
        /// 过期时间（分钟）
        /// </summary>
        [JsonPropertyName("accessTokenExpiration")]
        public int AccessTokenExpiration { get; set; }

    }
}
