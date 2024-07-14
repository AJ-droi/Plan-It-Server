using Plan_It.Enum;

namespace Plan_It.Dto.Task
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Category Category { get; set; }
        public Priority Priority { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public string StartTime { get; set; } = string.Empty;
    }
}