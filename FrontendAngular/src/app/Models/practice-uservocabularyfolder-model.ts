export interface PracticeUserFolderModel {
    id: number;
    lessonId: number;
    kanji?: string;           // Dấu ? tương đương với string? trong C#
    hiragana: string;
    romaji: string;
    meaning: string;
    textToSpeak?: string;
}