import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router'; // 👈 1. Import the Router provider
import { FacultySidebar } from './faculty-sidebar'; // Make sure this name exactly matches your class!

describe('FacultySidebar', () => {
  let component: FacultySidebar;
  let fixture: ComponentFixture<FacultySidebar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FacultySidebar], // Standalone components go in imports
      providers: [
        provideRouter([]) // 👈 2. Give the test a blank route map so routerLink doesn't crash
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(FacultySidebar);
    component = fixture.componentInstance;
    
    fixture.detectChanges(); // 👈 3. Force Angular to read the HTML during the test
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});