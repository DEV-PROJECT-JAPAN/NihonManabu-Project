// lesson.component.ts
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

// Services

import { LessonClientService } from '../../../Core/Services/lesson-client-service';
//Models
import { LessonModel } from '../../../Models/lesson-model';

@Component({
  selector: 'app-grammar-lesson',
  standalone: true,
  imports: [CommonModule, RouterModule],
  providers: [LessonClientService],
  templateUrl: './lessonsGrammar.html',
  styleUrls: ['./lessonsGrammar.css']
})
export class lessonsGrammar implements OnInit {
  levelId!: number;
  lessons: LessonModel[] = [];

  constructor(
    private _route: ActivatedRoute,
    private _router: Router,
    private _lessonService: LessonClientService,
    private _cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this._route.params.subscribe(params => {
      this.levelId = +params['levelId'];
      this._lessonService.getLessonsByLevelAsync(this.levelId).subscribe(res => {
        this.lessons = res;
        this._cdr.markForCheck();
      });
    });
  }

  selectLesson(lessonId: number): void {
    this._router.navigate(['/grammar/level', this.levelId, 'lesson', lessonId]);
  }
}