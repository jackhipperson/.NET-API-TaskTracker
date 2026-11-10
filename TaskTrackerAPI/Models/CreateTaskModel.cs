using System.ComponentModel.DataAnnotations;

namespace TaskTrackerAPI.Models
{
    public class CreateTaskModel
    {
        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}
