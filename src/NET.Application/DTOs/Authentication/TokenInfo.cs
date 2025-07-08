using System;

namespace NET.Application.DTOs.Authentication
{
    public class TokenInfo
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn => (int)(ExpiresAt - DateTime.UtcNow).TotalSeconds;
    }
}