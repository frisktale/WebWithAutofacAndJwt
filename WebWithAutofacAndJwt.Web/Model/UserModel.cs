using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebWithAutofacAndJwt.Web.Model;
/// <summary>
/// 输入的用户Model
/// </summary>
public class UserModel
{
    [Required]
    [JsonPropertyName("username")]
    public string UserName { get; set; }

    [Required]
    [JsonPropertyName("password")]
    public string Password { get; set; }
}
