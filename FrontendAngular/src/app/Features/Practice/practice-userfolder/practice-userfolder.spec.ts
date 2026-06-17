import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PracticeUserfolder } from './practice-userfolder';

describe('PracticeUserfolder', () => {
  let component: PracticeUserfolder;
  let fixture: ComponentFixture<PracticeUserfolder>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PracticeUserfolder]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PracticeUserfolder);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
