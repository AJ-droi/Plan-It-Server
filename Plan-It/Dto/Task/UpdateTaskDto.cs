using Plan_It.Enum;
using TaskStatus = Plan_It.Enum.TaskStatus;

namespace Plan_It.Dto.Task
{
    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Category Category { get; set; }
        public Priority Priority { get; set; }
        public TaskStatus TaskStatus { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; } = DateTime.Now;
        public string StartTime { get; set; } = string.Empty;
    }
}