using BackendAPI.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class FolderVocabulary : BaseModels
    {
        public int VocabularyId { get; set; }
        public int ListId { get; set; }
        // Dùng thẻ này để bảo EF: "Hãy nhìn vào cột ListId, đừng tự đẻ cột mới!"
        [ForeignKey("ListId")]
        public virtual UserFlashcardList UserFlashcardList { get; set; }

        // Tương tự, bảo EF: "Nhìn vào cột VocabularyId này này!"
        [ForeignKey("VocabularyId")]
        public virtual UserVocabulary UserVocabulary { get; set; }
    }
}
