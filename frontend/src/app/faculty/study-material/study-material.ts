import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { FacultySidebarComponent } from '../faculty-sidebar/faculty-sidebar';

@Component({
  selector: 'app-study-material',
  standalone: true,
  imports: [CommonModule, FormsModule, FacultySidebarComponent],
  templateUrl: './study-material.html',
  styleUrls: ['./study-material.css']
})
export class StudyMaterialComponent implements OnInit {
  viewMode: 'upload' | 'history' = 'upload';
  facultyId: string | null = null;
  feedback = { type: '', msg: '' };
  isLoading: boolean = false;

  // 🚀 New array to hold departments from DB
  departments: string[] = [];

  form = {
    department: '',
    subject: '',
    title: '',
    materialLink: ''
  };

  myMaterials: any[] = [];

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    const rawFid = sessionStorage.getItem("sessionFid");
    this.facultyId = rawFid ? rawFid.replace(/['"]/g, '').trim() : null;

    // 🚀 Fetch departments immediately
    this.fetchDepartments();
  }

  fetchDepartments(): void {
    this.http.get<string[]>("http://localhost:5139/api/Faculty/departments").subscribe({
      next: (data) => {
        this.departments = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error("Could not load departments", err)
    });
  }

  switchMode(mode: 'upload' | 'history'): void {
    this.viewMode = mode;
    this.feedback.msg = '';
    if (mode === 'history') {
      this.fetchMyMaterials();
    }
  }

  onSubmit(): void {
    if (!this.form.department || !this.form.subject || !this.form.title || !this.form.materialLink) {
      this.feedback = { type: 'danger', msg: 'Please fill all fields.' };
      return;
    }

    const payload = {
      FacultyId: this.facultyId,
      // Map frontend 'materialLink' to backend 'Link' to match CommonModels.cs
      Department: this.form.department,
      Subject: this.form.subject,
      Title: this.form.title,
      Link: this.form.materialLink
    };

    this.http.post("http://localhost:5139/api/Faculty/study-material", payload)
      .subscribe({
        next: (res: any) => {
          this.feedback = { type: 'success', msg: res.message };
          this.form = { department: '', subject: '', title: '', materialLink: '' };
          this.cdr.detectChanges();
        },
        error: () => this.feedback = { type: 'danger', msg: 'Failed to share material.' }
      });
  }

 fetchMyMaterials(): void {
    // 🚀 Check if facultyId exists in session before making the call
    if (this.facultyId === null || this.facultyId === undefined) {
        this.feedback = { type: 'danger', msg: 'Session error. Please re-login.' };
        return;
    }

    this.isLoading = true;

    // The URL must exactly match the [HttpGet] route defined in C#
    const url = `http://localhost:5139/api/Faculty/my-materials/${this.facultyId}`;

    this.http.get<any[]>(url).subscribe({
        next: (data) => {
            // Standardize mapping to ensure the HTML table keys match the DB keys
            this.myMaterials = data.map(m => ({
                postedOn: m.postedOn || m.PostedOn,
                department: m.department || m.Department,
                title: m.title || m.Title,
                link: m.link || m.Link
            }));
            this.isLoading = false;
            this.cdr.detectChanges();
        },
        error: (err) => {
            console.error("Fetch Error:", err);
            this.feedback = { type: 'danger', msg: 'The server could not find the history route.' };
            this.isLoading = false;
            this.cdr.detectChanges();
        }
    });
}
}
