using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebWithAutofacAndJwt.Web.Model
{
    public class User
    {
        [Required]
        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
