import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';

// ✅ Fixed import name to match the exported class
import { FacultyEditComponent } from './faculty-edit'; 

describe('FacultyEditComponent', () => {
  let component: FacultyEditComponent;
  let fixture: ComponentFixture<FacultyEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FacultyEditComponent],
      // ✅ Added providers so the test doesn't crash looking for the API or Router
      providers: [
        provideHttpClient(),
        provideRouter([]) 
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(FacultyEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges(); 
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});