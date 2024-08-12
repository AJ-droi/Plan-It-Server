using Microsoft.AspNetCore.Identity;
using Plan_It.Enum;

namespace Plan_It.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string Password { get; set; } = string.Empty;
        public string GoogleAuth { get; set; } = string.Empty;
        public string FacebookAuth { get; set; } = string.Empty;
        public DateTime BirthDay { get; set; } = DateTime.UtcNow;
        public string TaskInstruction{get; set;} = string.Empty;
        public Guid? GroupId {get; set;} = null;
        public Status UserStatus { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Group? Group {get; set;} 

    }
}