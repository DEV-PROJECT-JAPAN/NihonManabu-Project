using BackendAPI.Models.Base;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class Lesson : BaseModels
    {
        public int LevelId { get; set; }
        [ForeignKey("LevelId")]
        [ValidateNever]
        public virtual Level  Level { get; set; }

        [Required, Column(TypeName = "nvarchar(200)")]
        public string Title { get; set; }
        public int Order { get; set; } // thứ tự của bài học trong một level
        [ValidateNever]
        public virtual ICollection<Vocabulary> Vocabularies { get; set; }
        [ValidateNever]
        public virtual ICollection<Grammar> Grammars { get; set; }
    }
}
