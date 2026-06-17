namespace BackendAPI.DTOs
{
    public class PracticeDTO
    {
<<<<<<< Updated upstream
        //thuoc tinh: Id, Kanji, Hiragana, Romaji, Meaning
=======
>>>>>>> Stashed changes
        public int Id { get; set; }
        public int ListId { get; set; }
        public string Kanji { get; set; }
        public string Hiragana { get; set; }
        public string Romaji { get; set; }
        public string Meaning { get; set; }
<<<<<<< Updated upstream
=======
        public string? TextToSpeak => !string.IsNullOrEmpty(Hiragana) ? Hiragana : Kanji;
>>>>>>> Stashed changes
    }
}
