import { TestBed } from '@angular/core/testing';

import { LevelClientService } from './level-client-service';

describe('LevelClientService', () => {
  let service: LevelClientService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LevelClientService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
