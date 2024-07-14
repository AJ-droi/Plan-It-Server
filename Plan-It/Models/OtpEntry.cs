using System;

namespace Plan_It.Models
{
    public class OtpEntry
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public string? OtpCode { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
