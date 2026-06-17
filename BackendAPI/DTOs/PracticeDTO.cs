namespace BackendAPI.DTOs
{
    public class PracticeDTO
    {
        public int Id { get; set; }
        public int ListId { get; set; }
        public string Kanji { get; set; }
        public string Hiragana { get; set; }
        public string Romaji { get; set; }
        public string Meaning { get; set; }

        public string? TextToSpeak => !string.IsNullOrEmpty(Hiragana) ? Hiragana : Kanji;
    }
}
