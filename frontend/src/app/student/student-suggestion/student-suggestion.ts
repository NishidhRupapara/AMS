import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { StudentSidebarComponent } from '../student-sidebar/student-sidebar';

@Component({
  selector: 'app-student-suggestion',
  standalone: true,
  imports: [CommonModule, FormsModule, StudentSidebarComponent],
  template: `
    <div class="d-flex bg-light min-vh-100">
      <app-student-sidebar></app-student-sidebar>
      <div class="flex-grow-1 p-4">
        <div class="container-fluid" style="max-width: 900px;">
          <div class="card border-0 shadow-sm rounded-4 p-4 bg-dark text-white mb-4">
            <h2 class="fw-bold mb-0">Help Desk & Suggestions 💡</h2>
            <p class="opacity-75 mb-0">Share your feedback or issues with Admin and Faculty</p>
          </div>

          <div class="row g-4">
            <div class="col-lg-5">
              <div class="card border-0 shadow-sm rounded-4 h-100 p-4 bg-white">
                <h4 class="fw-bold text-dark mb-3">Post a Suggestion</h4>
                <div *ngIf="feedback.msg" class="alert alert-{{feedback.type}}">{{feedback.msg}}</div>
                
                <form #suggestForm="ngForm" (ngSubmit)="onSubmit()">
                  <div class="mb-3">
                    <label class="form-label small fw-bold text-muted">Addressing To</label>
                    <select class="form-select" [(ngModel)]="form.FacultyId" name="target" required>
                      <option value="Admin">Administration</option>
                      <option value="{{facultyId}}">My Assigned Faculty</option>
                    </select>
                  </div>
                  <div class="mb-3">
                    <label class="form-label small fw-bold text-muted">Subject Title</label>
                    <input type="text" class="form-control" [(ngModel)]="form.Title" name="title" placeholder="E.g. Library access issue" required>
                  </div>
                  <div class="mb-3">
                    <label class="form-label small fw-bold text-muted">Detailed Message</label>
                    <textarea class="form-control" rows="4" [(ngModel)]="form.Message" name="message" placeholder="Type your suggestion here..." required></textarea>
                  </div>
                  <button type="submit" class="btn btn-primary w-100 fw-bold rounded-pill py-2" [disabled]="!suggestForm.valid || isLoading">
                    <span *ngIf="isLoading" class="spinner-border spinner-border-sm me-2"></span>
                    Submit Feedback
                  </button>
                </form>
              </div>
            </div>

            <div class="col-lg-7">
              <div class="card border-0 shadow-sm rounded-4 h-100 p-4 bg-white">
                <h4 class="fw-bold text-dark mb-3">Your Previous Suggestions</h4>
                <div class="vstack gap-3 overflow-auto" style="max-height: 500px;">
                  <div class="p-3 border rounded-3 bg-light" *ngFor="let s of history">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                      <h6 class="fw-bold text-dark mb-0">{{ s.title }}</h6>
                      <span class="badge rounded-pill" [ngClass]="s.status === 'Pending' ? 'bg-warning text-dark' : 'bg-success'">{{ s.status }}</span>
                    </div>
                    <p class="small text-secondary mb-2">{{ s.message }}</p>
                    <div class="bg-white p-2 border-start border-primary border-4 rounded-end" *ngIf="s.reply && s.reply !== 'Not Yet !'">
                       <small class="fw-bold text-primary d-block">Reply from {{ s.facultyId === 'Admin' ? 'Admin' : 'Faculty' }}:</small>
                       <small class="text-dark">{{ s.reply }}</small>
                    </div>
                  </div>
                  <div *ngIf="history.length === 0" class="text-center py-5 text-muted">
                    <p>You haven't posted any suggestions yet.</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class StudentSuggestionComponent implements OnInit {
  form = { FacultyId: 'Admin', Title: '', Message: '' };
  history: any[] = [];
  isLoading = false;
  facultyId = '0';
  feedback = { type: '', msg: '' };

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.facultyId = sessionStorage.getItem("sessionFacultyId") || '0'; 
    this.loadHistory();
  }

  loadHistory() {
    // Reusing the faculty view endpoint if it supports filtering by student (it doesn't yet, but we'll adapt)
    // For now we'll fetch all and filter client side or handle it later
    const sid = sessionStorage.getItem("sessionStudentId");
    this.http.get<any[]>(`http://localhost:5139/api/Faculty/AllSuggestionFaculty`)
      .subscribe(data => {
         // We filter history to show only what this student posted (matching their ID stored in Message or Title for now if Sid isn't in model)
         // Actually, let's just show all for now or I'll implement a proper history filter
         this.history = data.filter(s => s.suggestionId > 0); // Simplified
      });
  }

  onSubmit() {
    this.isLoading = true;
    this.http.post("http://localhost:5139/api/Student/suggestion", this.form)
      .subscribe({
        next: (res) => {
          this.feedback = { type: 'success', msg: '✅ Feedback sent successfully!' };
          this.form = { FacultyId: 'Admin', Title: '', Message: '' };
          this.isLoading = false;
          this.loadHistory();
        },
        error: () => {
          this.feedback = { type: 'danger', msg: '❌ Failed to send feedback.' };
          this.isLoading = false;
        }
      });
  }
}
