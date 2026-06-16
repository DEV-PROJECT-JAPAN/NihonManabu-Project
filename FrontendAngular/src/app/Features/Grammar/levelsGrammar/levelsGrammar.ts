// level.component.ts
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { LevelClientService } from '../../../Core/Services/level-client-service';
import { LevelModel } from '../../../Models/level-model';

@Component({
  selector: 'app-grammar-level',
  standalone: true,
  imports: [CommonModule, RouterModule],
  providers: [LevelClientService],
  templateUrl: './levelsGrammar.html',
  styleUrls: ['./levelsGrammar.css']
})
export class levelsGrammar implements OnInit {
  levels: LevelModel[] = [];

  constructor(
    private _levelService: LevelClientService,
    private _router: Router,
    private _cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this._levelService.getLevelsAsync().subscribe(res => {
      this.levels = res;
      console.log(this.levels);
      this._cdr.markForCheck();
    });
  }

  selectLevel(id: number): void {
    this._router.navigate(['/grammar/level', id]);
  }
}