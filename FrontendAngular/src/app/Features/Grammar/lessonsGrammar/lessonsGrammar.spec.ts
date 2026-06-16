import { ComponentFixture, TestBed } from '@angular/core/testing';

import { lessonsGrammar } from './lessonsGrammar';

describe('lessonsGrammar', () => {
  let component: lessonsGrammar;
  let fixture: ComponentFixture<lessonsGrammar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [lessonsGrammar]
    })
      .compileComponents();

    fixture = TestBed.createComponent(lessonsGrammar);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
