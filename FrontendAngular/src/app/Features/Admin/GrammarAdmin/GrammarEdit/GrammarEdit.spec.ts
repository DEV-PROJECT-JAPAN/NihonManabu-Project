import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GrammarEdit } from './GrammarEdit';

describe('GrammarEdit', () => {
  let component: GrammarEdit;
  let fixture: ComponentFixture<GrammarEdit>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GrammarEdit]
    })
      .compileComponents();

    fixture = TestBed.createComponent(GrammarEdit);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
