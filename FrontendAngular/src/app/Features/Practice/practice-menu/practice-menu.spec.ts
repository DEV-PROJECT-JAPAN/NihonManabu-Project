import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PracticeMenu } from './practice-menu';

describe('PracticeMenu', () => {
  let component: PracticeMenu;
  let fixture: ComponentFixture<PracticeMenu>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PracticeMenu]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PracticeMenu);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
