import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StudentCenterComponent } from './student-center';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule } from '@angular/forms';

describe('StudentCenterComponent', () => {
  let component: StudentCenterComponent;
  let fixture: ComponentFixture<StudentCenterComponent>;

  beforeEach(async () => {
    // ✅ Use Object.defineProperty to create a robust mock for sessionStorage
    const store: { [key: string]: string } = {
      'sessionFid': '101',
      'sessionUsername': 'Test Faculty'
    };

    // This replaces the real getItem with our fake logic
    Object.defineProperty(window, 'sessionStorage', {
      value: {
        getItem: (key: string) => store[key] || null,
        setItem: (key: string, value: string) => store[key] = value,
        clear: () => {},
        removeItem: (key: string) => delete store[key],
        length: Object.keys(store).length,
        key: (index: number) => Object.keys(store)[index]
      },
      writable: true
    });

    await TestBed.configureTestingModule({
      imports: [
        StudentCenterComponent, 
        HttpClientTestingModule, 
        RouterTestingModule,
        FormsModule
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(StudentCenterComponent);
    component = fixture.componentInstance;
    
    // ✅ Detection happens after mocking is complete
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with facultyId from session', () => {
    // Ensure the component successfully read '101' from our fake storage
    expect(component.facultyId).toBe('101');
  });
});