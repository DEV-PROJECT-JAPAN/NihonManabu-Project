import { AnswerModel } from './answer-model';
export interface QuestionModel {
    id: number;
    grammarId: number;
    content: string;
    questionType: number;
    answers: AnswerModel[];
}