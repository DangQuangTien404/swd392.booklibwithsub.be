using System;

namespace BookLibwithSub.Service.Models
{
    /// <summary>
    /// Strongly typed settings for JWT token generation and validation.
    /// Values are bound from configuration ("Jwt" section).
    /// </summary>
    public class JwtOptions
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
    }
}
