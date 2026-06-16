import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GrammarCreate } from './GrammarCreate';

describe('GrammarCreate', () => {
  let component: GrammarCreate;
  let fixture: ComponentFixture<GrammarCreate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GrammarCreate]
    })
      .compileComponents();

    fixture = TestBed.createComponent(GrammarCreate);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
