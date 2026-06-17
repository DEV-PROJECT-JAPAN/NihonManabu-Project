import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PracticeGachavocabulary } from './practice-gachavocabulary';

describe('PracticeGachavocabulary', () => {
  let component: PracticeGachavocabulary;
  let fixture: ComponentFixture<PracticeGachavocabulary>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PracticeGachavocabulary]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PracticeGachavocabulary);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
