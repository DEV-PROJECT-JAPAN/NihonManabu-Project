export interface VocabularyModel {
    id: number;
    lessonId: number;
    kanji?: string;           // Dấu ? tương đương với string? trong C#
    hiragana: string;
    romaji: string;
    meaning: string;
    exampleSentence?: string; // Dấu ? vì có thể từ vựng không có câu ví dụ
    textToSpeak?: string;
}