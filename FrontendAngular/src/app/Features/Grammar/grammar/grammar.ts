// grammar-list.component.ts
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { GrammarClientService } from '../../../Core/Services/grammar-client-service';
import { GrammarModel } from '../../../Models/grammar-model';

@Component({
  selector: 'app-grammar-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  providers: [GrammarClientService],
  templateUrl: './grammar.html',
  styleUrls: ['./grammar.css']
})
export class GrammarList implements OnInit {
  levelId!: number;
  lessonId!: number;
  grammars: GrammarModel[] = [];

  constructor(
    private _route: ActivatedRoute,
    private _router: Router,
    private _grammarService: GrammarClientService,
    private _cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this._route.params.subscribe(params => {
      this.levelId = +params['levelId'];
      this.lessonId = +params['lessonId'];

      this._grammarService.getGrammarByLessonAsync(this.lessonId).subscribe(res => {
        this.grammars = res;
        this._cdr.markForCheck();
      });
    });
  }

  selectGrammar(id: number): void {
    this._router.navigate(['/grammar/level', this.levelId, 'lesson', this.lessonId, 'grammar', id]);
  }
}