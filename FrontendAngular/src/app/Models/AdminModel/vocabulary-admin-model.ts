export interface VocabularyAdminModel {
    id: number;
    lessonId: number;
    kanji?: string;
    hiragana: string;
    romaji: string;
    meaning: string;
    exampleSentence?: string;
    textToSpeak?: string;

    // Các trường đặc thù của Admin để theo dõi dữ liệu
    createdAt?: string;
    updatedAt?: string;
}