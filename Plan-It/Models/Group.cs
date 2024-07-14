namespace Plan_It.Models {
    public class Group {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UserId { get; set; } = String.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public List<GroupTask> GroupTasks { get; set; } = new List<GroupTask>();
}
}