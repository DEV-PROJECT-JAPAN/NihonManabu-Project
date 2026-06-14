using BackendAPI.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;




namespace BackendAPI.Models
{
    public class Level : BaseModels
    {
        [Required, Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; } // các mức độ học của tiếng nhật N5, N4...
        [Column(TypeName = "nvarchar(255)")]
        public string Description { get; set; }
        //virtual ở đây không dùng để kế thừa cho lớp con mà là kĩ thuật
        //Lazy Loading giúp tiện lợi trong việc lấy dữ liệu
        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
