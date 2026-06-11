// level.component.ts
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { VocabularyClientService } from '../../../Core/Services/vocabulary-client-service';
import { LevelModel } from '../../../Models/level-model';

@Component({
  selector: 'app-grammar-level',
  standalone: true,
  imports: [CommonModule, RouterModule],
  providers: [VocabularyClientService],
  templateUrl: './levels.html',
  styleUrls: ['./levels.css']
})
export class Levels implements OnInit {
  levels: LevelModel[] = [];

  constructor(
    private _vocabularyService: VocabularyClientService,
    private _router: Router,
    private _cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this._vocabularyService.getLevelsAsync().subscribe(res => {
      this.levels = res;
      this._cdr.markForCheck();
    });
  }

  selectLevel(id: number): void {
    this._router.navigate(['/grammar/level', id]);
  }
}