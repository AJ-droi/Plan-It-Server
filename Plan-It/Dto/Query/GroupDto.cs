using Plan_It.Enum;

namespace Plan_It.Dto.Query
{
    public class GroupDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty; // Filtering by GroupId
        public string SortField { get; set; } = "CreatedAt"; // Sorting field
        public string SortOrder { get; set; } = "asc";
    }
}


