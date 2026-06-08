// practice.component.ts
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { GrammarClientService } from '../../../Core/Services/grammar-client-service';
import { GrammarModel } from '../../../Models/grammar-model';
import { QuestionModel } from '../../../Models/question-model';
import { AnswerModel } from '../../../Models/answer-model';

@Component({
  selector: 'app-grammar-questions',
  standalone: true,
  imports: [CommonModule, RouterModule],
  providers: [GrammarClientService],
  templateUrl: './questions.html',
  styleUrls: ['./questions.css']
})
export class Questions implements OnInit {
  levelId!: number;
  lessonId!: number;
  grammarId!: number;

  questionType: number = -1;
  currentGrammar: GrammarModel | null = null;
  questions: QuestionModel[] = [];
  shuffledAnswersMap: { [questionId: number]: AnswerModel[] } = {};

  // Trạng thái bài tập
  quizEvaluated: { [questionId: number]: boolean } = {};
  quizSelectedAnswer: { [questionId: number]: AnswerModel } = {};
  quizCorrectAnswer: { [questionId: number]: AnswerModel } = {};
  arrangeAssembledWords: { [questionId: number]: string[] } = {};
  arrangePoolStatus: { [questionId: number]: { [wordKey: string]: boolean } } = {};
  arrangeChecked: { [questionId: number]: boolean } = {};
  arrangeIsCorrect: { [questionId: number]: boolean } = {};

  constructor(
    private _route: ActivatedRoute,
    private _grammarService: GrammarClientService,
    private _cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this._route.params.subscribe(params => {
      this.levelId = +params['levelId'];
      this.lessonId = +params['lessonId'];
      this.grammarId = +params['grammarId'];

      this._grammarService.getGrammarByIdAsync(this.grammarId).subscribe(res => {
        this.currentGrammar = res;
        this._cdr.markForCheck();
      });
    });
  }

  changeQuestionType(type: number): void {
    this.questionType = type;
    if (type === -1) return;

    this._grammarService.getQuestionsByGrammarAsync(this.grammarId, this.questionType).subscribe(res => {
      let rawQuestions: QuestionModel[] = JSON.parse(JSON.stringify(res));

      if (this.questionType === 0) {
        rawQuestions = this.shuffleArray(rawQuestions);
      }

      this.shuffledAnswersMap = {};
      rawQuestions.forEach(q => {
        this.shuffledAnswersMap[q.id] = q.answers && q.answers.length > 0
          ? this.shuffleArray(q.answers)
          : [];
      });

      this.questions = rawQuestions;
      this.resetExerciseStates();
      this._cdr.markForCheck();
    });
  }

  private shuffleArray<T>(array: T[]): T[] {
    const clone = [...array];
    for (let i = clone.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [clone[i], clone[j]] = [clone[j], clone[i]];
    }
    return clone;
  }

  private resetExerciseStates(): void {
    this.quizEvaluated = {};
    this.quizSelectedAnswer = {};
    this.quizCorrectAnswer = {};
    this.arrangeAssembledWords = {};
    this.arrangePoolStatus = {};
    this.arrangeChecked = {};
    this.arrangeIsCorrect = {};

    this.questions.forEach(q => {
      if (q.questionType === 2) {
        this.arrangeAssembledWords[q.id] = [];
        this.arrangePoolStatus[q.id] = {};
        this.arrangeChecked[q.id] = false;
        this.arrangeIsCorrect[q.id] = false;
      }
    });
  }

  // Toàn bộ Logic xử lý trắc nghiệm và sắp xếp câu (giữ nguyên gốc)
  public selectQuizAnswer(question: QuestionModel, answer: AnswerModel): void {
    if (this.quizEvaluated[question.id]) return;
    this.quizEvaluated[question.id] = true;
    this.quizSelectedAnswer[question.id] = answer;
    const correct = question.answers.find(a => a.isCorrect);
    if (correct) this.quizCorrectAnswer[question.id] = correct;
  }

  public handleWordClick(questionId: number, wordText: string, poolIndex: number): void {
    if (this.arrangeChecked[questionId]) return;
    const key = `${wordText}_${poolIndex}`;
    if (this.arrangePoolStatus[questionId][key]) return;
    this.arrangePoolStatus[questionId][key] = true;
    this.arrangeAssembledWords[questionId].push(wordText);
  }

  public removeAssembledWord(questionId: number, wordText: string, assembledIndex: number): void {
    if (this.arrangeChecked[questionId]) return;
    this.arrangeAssembledWords[questionId].splice(assembledIndex, 1);
    const key = `${wordText}_${assembledIndex}`;
    this.arrangePoolStatus[questionId][key] = false;
  }

  public checkArrangeAnswer(question: QuestionModel): void {
    this.arrangeChecked[question.id] = true;
    const correctSentence = this.getCorrectSentenceText(question);
    const userSentence = this.arrangeAssembledWords[question.id].join(' ').trim();
    this.arrangeIsCorrect[question.id] = (userSentence === correctSentence);
  }

  public resetArrangeQuestion(questionId: number): void {
    this.arrangeAssembledWords[questionId] = [];
    this.arrangePoolStatus[questionId] = {};
    this.arrangeChecked[questionId] = false;
    this.arrangeIsCorrect[questionId] = false;
  }

  public getCorrectSentenceText(question: QuestionModel): string {
    return question.answers.slice().sort((a, b) => a.displayOrder.localeCompare(b.displayOrder)).map(a => a.text).join(' ').trim();
  }

  public getFilledSentence(content: string, text: string | undefined): string {
    if (!text) return content;
    return content.replace(/\[\s*\]/g, text).replace(/_+/g, text);
  }

  public speakJapanese(text: string): void {
    if ('speechSynthesis' in window) {
      const cleanText = text.replace(/\[\s*\]/g, '').replace(/_+/g, '');
      const utterance = new SpeechSynthesisUtterance(cleanText);
      utterance.lang = 'ja-JP';
      utterance.rate = 0.85;
      window.speechSynthesis.speak(utterance);
    }
  }
}