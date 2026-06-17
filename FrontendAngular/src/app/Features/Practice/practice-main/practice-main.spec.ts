import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PracticeMain } from './practice-main';

describe('PracticeMain', () => {
  let component: PracticeMain;
  let fixture: ComponentFixture<PracticeMain>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PracticeMain]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PracticeMain);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
