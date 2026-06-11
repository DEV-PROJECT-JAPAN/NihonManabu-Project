import { QuestionModel } from './question-model';
export interface GrammarModel {
    id: number;
    lessonId: number;
    structure: string;
    explanation: string;
    questions: QuestionModel[];
}