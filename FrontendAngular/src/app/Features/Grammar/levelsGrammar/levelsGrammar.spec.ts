import { ComponentFixture, TestBed } from '@angular/core/testing';

import { levelsGrammar } from './levelsGrammar';

describe('levelsGrammar', () => {
  let component: levelsGrammar;
  let fixture: ComponentFixture<levelsGrammar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [levelsGrammar]
    })
      .compileComponents();

    fixture = TestBed.createComponent(levelsGrammar);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
