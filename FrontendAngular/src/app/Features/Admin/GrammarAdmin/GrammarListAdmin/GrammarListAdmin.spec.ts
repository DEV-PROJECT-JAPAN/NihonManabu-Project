import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GrammarListAdmin } from './GrammarListAdmin';

describe('GrammarListAdmin', () => {
  let component: GrammarListAdmin;
  let fixture: ComponentFixture<GrammarListAdmin>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GrammarListAdmin]
    })
      .compileComponents();

    fixture = TestBed.createComponent(GrammarListAdmin);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
