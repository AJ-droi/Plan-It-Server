using Plan_It.Enum;
using TaskStatus = Plan_It.Enum.TaskStatus;

namespace Plan_It.Models
{
    public class Task
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Category Category { get; set; }
        public Priority Priority { get; set; }
        public TaskStatus TaskStatus { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; } = DateTime.UtcNow;
        public string StartTime { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
         public List<GroupTask> GroupTasks { get; set; } = new List<GroupTask>();
    }
}
