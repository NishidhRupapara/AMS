import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { FacultySidebarComponent } from '../faculty-sidebar/faculty-sidebar';

@Component({
  selector: 'app-assignment',
  standalone: true,
  imports: [CommonModule, FormsModule, FacultySidebarComponent],
  templateUrl: './assignment.html',
  styleUrls: ['./assignment.css']
})
export class AssignmentComponent implements OnInit {
  viewMode: 'post' | 'history' = 'post';
  facultyId: string | null = null;
  feedback = { type: '', msg: '' };
  isLoading: boolean = false;

  form = {
    department: '',
    subject: '',
    title: '',
    description: '',
    dueDate: '',
    referenceLink: ''
  };

  departments: any[] = [];
  myAssignments: any[] = [];

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    const rawFid = sessionStorage.getItem("sessionFid");
    this.facultyId = rawFid ? rawFid.replace(/['"]/g, '').trim() : null;
    this.fetchDepartments();
  }

  fetchDepartments(): void {
    this.http.get<any[]>("http://localhost:5139/api/Departments/Dall")
      .subscribe({
        next: (data) => {
          this.departments = data;
          this.cdr.detectChanges();
        },
        error: (err) => console.error("Error loading departments for assignment POST", err)
      });
  }

  switchMode(mode: 'post' | 'history'): void {
    this.viewMode = mode;
    this.feedback.msg = '';
    if (mode === 'history') {
      this.fetchMyAssignments();
    }
  }

  onSubmit(): void {
    if (!this.form.department || !this.form.subject || !this.form.title || !this.form.dueDate) {
      this.feedback = { type: 'danger', msg: 'Please fill all required fields.' };
      return;
    }

    const payload = {
      FacultyId: this.facultyId,
      ...this.form
    };

    this.http.post("http://localhost:5139/api/Faculty/post-assignment", payload)
      .subscribe({
        next: (res: any) => {
          this.feedback = { type: 'success', msg: res.message };
          this.form = { department: '', subject: '', title: '', description: '', dueDate: '', referenceLink: '' }; 
          this.cdr.detectChanges();
        },
        error: () => this.feedback = { type: 'danger', msg: 'Failed to post assignment.' }
      });
  }

  fetchMyAssignments(): void {
    this.isLoading = true;
    this.http.get<any[]>(`http://localhost:5139/api/Faculty/my-assignments/${this.facultyId}`)
      .subscribe({
        next: (data) => {
          this.myAssignments = data;
          this.isLoading = false;
          this.cdr.detectChanges();
        },
        error: () => {
          this.feedback = { type: 'danger', msg: 'Failed to load history.' };
          this.isLoading = false;
        }
      });
  }
}