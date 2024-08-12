using System.ComponentModel.DataAnnotations;

namespace Plan_It.Dto.Authentication
{
    public class AssignTaskInstructionDto
    {
        [Required]
        public string? instruction { get; set; }

 
    }
}