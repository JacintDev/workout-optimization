import { TestBed } from '@angular/core/testing';

import { PulsemeasureService } from './pulsemeasure.service';

describe('PulsemeasureService', () => {
  let service: PulsemeasureService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PulsemeasureService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
