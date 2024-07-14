namespace Plan_It.Dto.Group{
    public class CreateGroupDto{
        public string? Name { get; set; }
        public List<string>? Members { get; set; } = new List<string>();
    }
}