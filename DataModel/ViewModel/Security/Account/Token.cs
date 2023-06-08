using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataModel.ViewModel.Security.Account
{
    public class Token
    {
        [JsonPropertyName("refreshToken")]
        [Required]
        public string? RefreshToken { get; set; }
    }
}
