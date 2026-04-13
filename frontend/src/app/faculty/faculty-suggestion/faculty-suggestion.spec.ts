import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FacultySuggestion } from './faculty-suggestion';

describe('FacultySuggestion', () => {
  let component: FacultySuggestion;
  let fixture: ComponentFixture<FacultySuggestion>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FacultySuggestion],
    }).compileComponents();

    fixture = TestBed.createComponent(FacultySuggestion);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
