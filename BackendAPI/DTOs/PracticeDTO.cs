namespace BackendAPI.DTOs
{
    public class PracticeDTO
    {
        //thuoc tinh: Id, Kanji, Hiragana, Romaji, Meaning
        public int Id { get; set; }
        public int IdLesson { get; set; }
        public string Kanji { get; set; }
        public string Hiragana { get; set; }
        public string Romaji { get; set; }
        public string Meaning { get; set; }
    }
}
