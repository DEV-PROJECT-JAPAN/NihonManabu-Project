import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuestionEdit } from './QuestionEdit';

describe('Edit', () => {
  let component: QuestionEdit;
  let fixture: ComponentFixture<QuestionEdit>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuestionEdit]
    })
      .compileComponents();

    fixture = TestBed.createComponent(QuestionEdit);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
