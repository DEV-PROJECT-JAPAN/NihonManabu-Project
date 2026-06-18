import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Kanji } from './kanji';

describe('Kanji', () => {
  let component: Kanji;
  let fixture: ComponentFixture<Kanji>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Kanji]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Kanji);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
