import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { AdminSidebarComponent } from '../admin-sidebar/admin-sidebar';

@Component({
  selector: 'app-admin-assignments',
  standalone: true,
  imports: [CommonModule, AdminSidebarComponent],
  templateUrl: './admin-assignments.html',
  styleUrls: ['./admin-assignments.css']
})
export class AdminAssignmentsComponent implements OnInit {
  assignments: any[] = [];
  isLoading: boolean = true;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.fetchAssignments();
  }

  fetchAssignments(): void {
    this.http.get<any[]>("http://localhost:5139/api/Admin/all-assignments").subscribe({
      next: (data) => {
        this.assignments = data;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  deleteAssignment(id: string): void {
    if (confirm("Are you sure you want to delete this assignment?")) {
      this.http.delete(`http://localhost:5139/api/Admin/assignment/${id}`).subscribe({
        next: () => {
          this.assignments = this.assignments.filter(a => a.id !== id);
          alert("Assignment deleted successfully!");
        },
        error: () => alert("Failed to delete assignment.")
      });
    }
  }
}
