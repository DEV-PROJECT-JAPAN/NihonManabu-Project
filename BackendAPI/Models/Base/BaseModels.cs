using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Models.Base
{
    public abstract class BaseModels
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
