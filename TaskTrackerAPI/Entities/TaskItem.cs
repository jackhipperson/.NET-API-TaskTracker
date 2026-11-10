using System.ComponentModel.DataAnnotations;

namespace TaskTrackerAPI.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
    }
}
