namespace Plan_It.Models{
    public class GroupTask
    {
        public Guid GroupId { get; set; }
        public Group? Group { get; set; }

        public Guid TaskId { get; set; }
        public Task? Task { get; set; }
    }

}