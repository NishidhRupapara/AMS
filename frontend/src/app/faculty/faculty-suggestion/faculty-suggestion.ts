import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { FacultySidebarComponent } from '../faculty-sidebar/faculty-sidebar';

@Component({
  selector: 'app-faculty-suggestion',
  standalone: true,
  imports: [CommonModule, FormsModule, FacultySidebarComponent],
  templateUrl: './faculty-suggestion.html',
  styleUrls: ['./faculty-suggestion.css']
})
export class FacultySuggestionComponent implements OnInit {
  // Form Model
  form = { title: '', message: '' };
  
  // State Management
  mySuggestions: any[] = [];
  isLoading: boolean = false;
  viewMode: 'add' | 'list' = 'add'; // Toggle between "Add" and "History"
  
  facultyId: string | null = null;
  feedback = { type: '', msg: '' };

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    // Retrieve the Faculty ID from session
    const rawFid = sessionStorage.getItem("sessionFid");
    this.facultyId = rawFid ? rawFid.replace(/['"]/g, '').trim() : null;

    if (!this.facultyId) {
      this.feedback = { type: 'danger', msg: 'Session expired. Please login again.' };
    }
  }

  // Toggle between Add Form and History List
  switchMode(mode: 'add' | 'list'): void {
    this.viewMode = mode;
    this.feedback.msg = ''; // Clear alerts when switching
    if (mode === 'list') {
      this.fetchMySuggestions();
    }
  }

  // ✅ POST: Connects to your [HttpPost("suggestion")]
  onSubmit(): void {
    if (!this.form.title.trim() || !this.form.message.trim()) return;

    const payload = {
      facultyId: this.facultyId,
      title: this.form.title,
      message: this.form.message
    };

    this.http.post("http://localhost:5139/api/Faculty/suggestion", payload)
      .subscribe({
        next: (res: any) => {
          this.feedback = { type: 'success', msg: res.message || '✅ Submitted Successfully!' };
          this.form = { title: '', message: '' };
          this.cdr.detectChanges();
        },
        error: () => {
          this.feedback = { type: 'danger', msg: '❌ Error: Could not reach the server.' };
        }
      });
  }

  // ✅ GET: Connects to your [HttpGet("ViewSuggestion")]
  fetchMySuggestions(): void {
    this.isLoading = true;
    this.http.get<any[]>(`http://localhost:5139/api/Faculty/ViewSuggestion?Facultyid=${this.facultyId}`)
      .subscribe({
        next: (data) => {
          this.mySuggestions = data;
          this.isLoading = false;
          this.cdr.detectChanges();
        },
        error: () => {
          this.isLoading = false;
          this.feedback = { type: 'danger', msg: 'Failed to load your history.' };
        }
      });
  }
}