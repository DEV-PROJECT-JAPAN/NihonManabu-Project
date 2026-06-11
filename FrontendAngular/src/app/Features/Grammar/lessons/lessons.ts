// lesson.component.ts
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { VocabularyClientService } from '../../../Core/Services/vocabulary-client-service';
import { LessonModel } from '../../../Models/lesson-model';

@Component({
  selector: 'app-grammar-lesson',
  standalone: true,
  imports: [CommonModule, RouterModule],
  providers: [VocabularyClientService],
  templateUrl: './lessons.html',
  styleUrls: ['./lessons.css']
})
export class LessonComponent implements OnInit {
  levelId!: number;
  lessons: LessonModel[] = [];

  constructor(
    private _route: ActivatedRoute,
    private _router: Router,
    private _vocabularyService: VocabularyClientService,
    private _cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this._route.params.subscribe(params => {
      this.levelId = +params['levelId'];
      this._vocabularyService.getLessonsAsync(this.levelId).subscribe(res => {
        this.lessons = res;
        this._cdr.markForCheck();
      });
    });
  }

  selectLesson(lessonId: number): void {
    this._router.navigate(['/grammar/level', this.levelId, 'lesson', lessonId]);
  }
}