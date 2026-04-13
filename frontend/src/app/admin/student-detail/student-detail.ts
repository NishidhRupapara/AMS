import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { AdminSidebarComponent } from '../admin-sidebar/admin-sidebar';

@Component({
  selector: 'app-student-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, AdminSidebarComponent],
  template: `
  <div class="d-flex bg-light min-vh-100">
    <app-admin-sidebar></app-admin-sidebar>
    <div class="main-content flex-grow-1 p-4">

      <div class="d-flex justify-content-between align-items-center mb-4 bg-white p-3 shadow-sm rounded">
        <h3 class="mb-0 text-info fw-bold">Student Details</h3>
        <a routerLink="/view-students" class="btn btn-secondary fw-bold">🔙 Back to List</a>
      </div>

      <div *ngIf="isLoading" class="text-center py-5">
        <div class="spinner-border text-info" role="status"></div>
        <p class="mt-2 fw-bold text-muted">Fetching student details...</p>
      </div>

      <div class="card shadow-sm border-0 rounded-3 p-4" *ngIf="!isLoading && student">
        <h4 class="text-center text-success fw-bold mb-4">
          {{ student.fname || student.Fname }} {{ student.lname || student.Lname }}
        </h4>

        <table class="table table-bordered table-striped align-middle">
          <tbody>
            <tr><td class="fw-bold w-25">Student ID</td><td>{{ student.sid || student.Sid }}</td></tr>
            <tr><td class="fw-bold">Faculty ID</td><td>{{ student.faculty_Id || student.Faculty_Id }}</td></tr>
            <tr><td class="fw-bold">Gender</td><td>{{ student.gender || student.Gender }}</td></tr>
            <tr><td class="fw-bold">Department</td><td>{{ student.department || student.Department }}</td></tr>
            <tr><td class="fw-bold">Date of Birth</td><td>{{ (student.dob || student.Dob) | date:'longDate' }}</td></tr>
            <tr><td class="fw-bold">Date of Admission</td><td>{{ (student.dateOfAdmission || student.DateOfAdmission || student.doa) | date:'longDate' }}</td></tr>
            <tr><td class="fw-bold">Email</td><td>{{ student.email_Id || student.Email_Id }}</td></tr>
            <tr><td class="fw-bold">Mobile</td><td>{{ student.mobile_No || student.Mobile_No }}</td></tr>
            <tr><td class="fw-bold">Address</td><td>{{ student.address || student.Address }}</td></tr>
            <tr><td class="fw-bold">Parent Name</td><td>{{ student.parentName || student.ParentName || student.parent_Name || student.Parent_Name }}</td></tr>
            <tr><td class="fw-bold">Parent Mobile</td><td>{{ student.parentMobile || student.ParentMobile || student.parent_Mobile || student.Parent_Mobile }}</td></tr>
            <tr><td class="fw-bold">Parent Email</td><td>{{ student.parentEmail || student.ParentEmail || student.parent_Email || student.Parent_Email }}</td></tr>
          </tbody>
        </table>

        <div class="text-center mt-4">
          <a [routerLink]="['/student/edit', student.sid || student.Sid]" class="btn btn-warning fw-bold px-4 shadow-sm">✏️ Edit Student</a>
        </div>
      </div>

      <div class="alert alert-danger text-center" *ngIf="!isLoading && !student">
        ⚠️ Student not found or error loading data.
      </div>

    </div>
  </div>
  `
})
export class StudentDetailComponent implements OnInit {
  student: any = null;
  isLoading: boolean = true; // Added loading state

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private cdr: ChangeDetectorRef // Injected Change Detector
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.fetchStudentDetails(id);
    } else {
      this.isLoading = false;
    }
  }

  fetchStudentDetails(id: string) {
    this.isLoading = true;
    this.http.get(`http://localhost:5139/api/Student/${id}`).subscribe({
      next: (data) => {
        this.student = data;
        this.isLoading = false;
        this.cdr.detectChanges(); // Force UI update after data arrives
      },
      error: (err) => {
        console.error("Error loading student:", err);
        alert("Error loading student details");
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }
}
